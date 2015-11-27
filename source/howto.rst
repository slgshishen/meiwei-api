接入指南
========================================


消息加解密与安全
------------------

安全体系
^^^^^^^^^^^^^

美味开放平台采用了如下方式对数据进行了安全处理，具体如下：

开发者申请得到的appKey，经过SHA-1 128位摘要算法后取前128位生成摘要签名sign（sign应经过Base64转码成字符串），(sign+token)继续经过SHA-1 128位摘要算法后取前128位生成真正的AES加密秘钥 key S2（S2应经过Base64转码成字符串），开发者在发给美味的数据中应包括sign、unix时间戳ts（秒），且整体数据经过S2的AES加密。美味侧，首先会进行解密， 获取sign和ts，并且验证sign是否能生成S2且ts是否在10min中内，以此来判断该请求是否有效。

美味推送给开发者的信息也经过S2加密，但是无sign和ts；开发者在response的时候应该参照上述的方式进行。


加解密算法
^^^^^^^^^^^^^

美味开放平台采用AES对称加密算法对推送给开发者的消息体进行加密，加解密所用的密钥是S2。 开发者用此密钥对收到的密文消息体进行解密，回复消息体也用此密钥加密，且应带上sign和ts。

加解密使用对称密码的AES规范，加密后的结果采用Base 64进行表示，密钥本身也使用Base 64的表示，涉及中文等字符使用utf-8字符集。

美味开放平台可以接受第63和第64个字符为+/的Base 64和url-safe的-_的Base 64，给出的密文将使用url-safe的-_的Base 64。

Base 64的内容放入content字段进行传输，收到content字段时，可将字段内容用Base64解析，将解析结果按照AES规范和对应的密钥（S2）进行解密。

加解密实现机制
^^^^^^^^^^^^^^^^^

.. image:: _static/images/encryption-in-action.png

图例形象化说明了解密和加密的过程。开发者密钥和content字段值是Base 64表示的字符串，经过解码后，对应的结果为两个八位数组。

八位数组中一个是aes key，另一个是aes加密的内容。依循AES规范，用aes key解密aes的加密内容得到对应的明文。

加密的时候，将明文当作八位数组，依循AES规范，用aes key加密得到aes的加密内容，然后将aes的加密内容用Base 64表示为字符串得到对应的密文。


示例代码(Java)::

    import org.apache.commons.codec.binary.Base64;
    import javax.crypto.Cipher;
    import javax.crypto.SecretKey;
    import javax.crypto.spec.SecretKeySpec;

    public class SimpleStringCypher {
        private byte[] linebreak = {};
        private String secret;
        private SecretKey key;
        private Cipher cipher;
        private Base64 coder;

        public SimpleStringCypher(String secret) {
            try {
                coder = new Base64(32, linebreak, true);
                //secret为密钥 S2
                byte[] secrets = coder.decode(secret);
                key = new SecretKeySpec(secrets, "AES");
                cipher = Cipher.getInstance("AES/ECB/PKCS5Padding", "SunJCE");
            } catch (Throwable t) {
                t.printStackTrace();
            }
        }

        //对推送消息中content进行加密
        public synchronized String encrypt(String plainText) throws Exception {
            cipher.init(Cipher.ENCRYPT_MODE, key);
            byte[] cipherText = cipher.doFinal(plainText.getBytes());
            return new String(coder.encode(cipherText));
        }

        //对返回结果的content进行解密
        public synchronized String decrypt(String codedText) throws Exception {
            byte[] encypted = coder.decode(codedText.getBytes());
            cipher.init(Cipher.DECRYPT_MODE, key);
            byte[] decrypted = cipher.doFinal(encypted);
            return new String(decrypted, "UTF-8");
        }
    }
                    
示例代码(.Net)::

    using System;
    using System.Text;
    using System.Security.Cryptography;
    using System.Configuration;

    namespace ConsoleApplication1
    {
        class SimpleStringCypher
        {
            private RijndaelManaged RM;

            public SimpleStringCypher(String secret)
            {
                //secret为密钥 S2
                var keyBytes = PrepareAesKey(secret);

                RM = new System.Security.Cryptography.RijndaelManaged
                {
                    Mode = System.Security.Cryptography.CipherMode.ECB,
                    Padding = System.Security.Cryptography.PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128,
                    Key = keyBytes,
                    IV = keyBytes
                };
            }

            public string Encrypt(string plaintext)
            {
                if (string.IsNullOrEmpty(plaintext)) return null;
                Byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

                ICryptoTransform cTransform = RM.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(plaintextBytes,
                            0, plaintextBytes.Length);

                return URLSafeBase64Reflow(Convert.ToBase64String(resultArray,
                            0, resultArray.Length));
            }

            public string Decrypt(string codedText)
            {
                if (string.IsNullOrEmpty(codedText)) return null;
                Byte[] toDeryptArray = Convert.FromBase64String(
                            AutomaticallyPad(NormalBase64Reflow(codedText)));

                ICryptoTransform cTransform = RM.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toDeryptArray,
                            0, toDeryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }

            private static string AutomaticallyPad(string base64)
            {
                return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
            }

            private static string URLSafeBase64Reflow(string base64)
            {
                return base64.Replace("=", String.Empty).Replace('+', '-').Replace('/', '_');
            }

            private static string NormalBase64Reflow(string base64)
            {
                return base64.Replace("=", String.Empty).Replace('-', '+').Replace('_', '/');
            }

            private static byte[] PrepareAesKey(string key)
            {
                Byte[] keyBinary = Convert.FromBase64String(
                            AutomaticallyPad(NormalBase64Reflow(key)));
                var keyBytes = new byte[16];
                Array.Copy(keyBinary, keyBytes, Math.Min(keyBytes.Length, keyBinary.Length));
                return keyBytes;
            }
        }
    }

                    
示例代码(php)::

    <?php


    class SimpleStringCypher {

       public static function encrypt($input, $key){
        //key为密钥 S2
        $key = SimpleStringCypher::reflowNormalBase64($key);
        $size  = mcrypt_get_block_size(MCRYPT_RIJNDAEL_128, MCRYPT_MODE_ECB);
        $input = SimpleStringCypher::pkcs5_pad($input, $size);
        $td    = mcrypt_module_open(MCRYPT_RIJNDAEL_128, '', MCRYPT_MODE_ECB, '');
        $iv    = mcrypt_create_iv(mcrypt_enc_get_iv_size($td), MCRYPT_RAND);
        mcrypt_generic_init($td, base64_decode($key), $iv);
        $data = mcrypt_generic($td, $input);
        mcrypt_generic_deinit($td);
        mcrypt_module_close($td);
        $data = base64_encode($data);
        $data = SimpleStringCypher::reflowURLSafeBase64($data);
        return $data;
      }

      public static function decrypt($sStr, $sKey){
        //sKey为密钥 S2
        $sStr = SimpleStringCypher::reflowNormalBase64($sStr);
        $sKey = SimpleStringCypher::reflowNormalBase64($sKey);
        $decrypted = mcrypt_decrypt(MCRYPT_RIJNDAEL_128,
                            base64_decode($sKey), base64_decode($sStr),
                            MCRYPT_MODE_ECB);
        $dec_s     = strlen($decrypted);
        $padding   = ord($decrypted[$dec_s - 1]);
        $decrypted = substr($decrypted, 0, -$padding);
        return $decrypted;
      }

      private static function reflowURLSafeBase64($str){
        $str=str_replace("/","_",$str);
        $str=str_replace("+","-",$str);
        return $str;
      }

      private static function reflowNormalBase64($str){
        $str=str_replace("_","/",$str);
        $str=str_replace("-","+",$str);
        return $str;
      }

      private static function pkcs5_pad($text, $blocksize){
        $pad = $blocksize - (strlen($text) % $blocksize);
        return $text . str_repeat(chr($pad), $pad);
      }

    }
    ?>

                    
示例代码(nodejs)::

    var crypto = require('crypto');

    var ALGORITHM = 'aes-128-ecb';

    function fromUrlSafe(str) {
        return str.replace(/-/g, "+").replace(/_/g, "/");
    }
    function toUrlSafe(str) {
        return str.replace(/\+/g, "-").replace(/\//g, "_");
    }

    function SimpleStringCipher(secretStr) {
        //secretStr为密钥 S2
        this.secret = new Buffer(fromUrlSafe(secretStr), 'base64');
    }
    SimpleStringCipher.prototype = {
        encrypt: function (data) {
            if (!data instanceof Buffer) {
                data = new Buffer('' + data);
            }
            var cipher = crypto.createCipher(ALGORITHM, this.secret);
            var res = cipher.update(data);
            var rest = cipher.final();
            return toUrlSafe(Buffer.concat([res, rest]).toString('base64'));
        },
        decrypt: function (data) {
            data = new Buffer(fromUrlSafe(data), 'base64');
            var cipher = crypto.createDecipher(ALGORITHM, this.secret);
            var res = cipher.update(data);
            var rest = cipher.final();
            return Buffer.concat([res, rest]).toString();
        }
    };

    module.exports = SimpleStringCipher;

    // 用法示例
    var cipher = new SimpleStringCipher('YXNkZmFzZGZhc2RmYXNkCg==');
    var src = 'This is a\ntest case\n';
    var encoded = cipher.encrypt(src);
    var decoded = cipher.decrypt(encoded);

    console.log(src);
    console.log(encoded);
    console.log(decoded);



发送/接收消息
------------------

发送/接收消息请求说明
^^^^^^^^^^^^^^^^^^^^^^^^^^

开发者向点评发送消息或者点评向开发者推送消息均采用以下参数及返回形式

HTTP请求方式：POST (form-data)

返回数据格式：JSON
                
参数说明
^^^^^^^^^^^^^^^^^^^^^^^^^^

========== =========== =======================================================================================
参数        类型         描述
========== =========== =======================================================================================
token*     String       点评提供给您的开发者token
content*   jsonString   S2加密后的业务数据，格式见content说明
version    String       用于标识调用的点评业务方服务的版本，默认是v1.0.0版本，此版本号由点评提供，和调用的业务相关
========== =========== =======================================================================================

content说明
^^^^^^^^^^^^^^^^^^^^^^^^^^

========== =========== =======================================================================================
参数        类型         描述
========== =========== =======================================================================================
content*    String      业务数据
sign*       String      appKey经过SHA1摘要加密且经过base64转码生成
ts*         long        unix时间戳（秒）
========== =========== =======================================================================================

返回结果说明
^^^^^^^^^^^^^^^^^^^^^^^^^^

========== =========== =======================================================================================
字段        类型         描述
========== =========== =======================================================================================
code        int        返回码 200成功，非200失败 参见
msg         string     返回消息
content     string     加密后的业务返回数据
id          long       请求id
properties  json       附加信息map转换成的json
========== =========== =======================================================================================

返回码和问题诊断
------------------

接入上线步骤说明
------------------