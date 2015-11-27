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