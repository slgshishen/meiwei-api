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