
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Text;
using System.Security.Cryptography;

public class SellerResponsitory : ISellerResponsitory
{
    private readonly DatabaseContext _context;
    public SellerResponsitory(DatabaseContext context)
    {
        _context = context;
    }

    // Phương thức giải mã
    public string decrypt(string encrypted)
    {
        string hash = "cong@gmail.com";
        byte[] data = Convert.FromBase64String(encrypted);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider();
        tripDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
        tripDES.Mode = CipherMode.ECB;
        ICryptoTransform transform = tripDES.CreateDecryptor();
        byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
        return UTF8Encoding.UTF8.GetString(result);
    }

    // Phương thức mã hoá
    public string encrypt(string decryted)
    {
        string hash = "cong@gmail.com";
        byte[] data = UTF8Encoding.UTF8.GetBytes(decryted);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider();
        tripDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
        tripDES.Mode = CipherMode.ECB;
        ICryptoTransform transform = tripDES.CreateEncryptor();
        byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

        return Convert.ToBase64String(result);
    }

    public bool changePasswordSellerAccount(int sellerID, string password)
    {
        SqlParameter sellerIDParam = new SqlParameter("@PK_iSellerID", sellerID);
        SqlParameter passwordParam = new SqlParameter("@sSellerPassword", password);
        _context.Database.ExecuteSqlRaw("EXEC sp_ChangePasswordSellerAccount @PK_iSellerID, @sSellerPassword", sellerIDParam, passwordParam);
        return true;
    }

    public IEnumerable<Seller> checkSellerAccountByIDAndPass(int sellerID, string password)
    {
        SqlParameter sellerIDParam = new SqlParameter("@PK_iSellerID", sellerID);
        SqlParameter passwordParam = new SqlParameter("@sSellerPassword", password);
        return _context.Sellers.FromSqlRaw("EXEC sp_CheckSellerAccountByIDAndPass @PK_iSellerID, @sSellerPassword", sellerIDParam, passwordParam);
    }

    public IEnumerable<Seller> getPasswordSellerAccountByPhone(string phone)
    {
        SqlParameter phoneParam = new SqlParameter("@sSellerPhone", phone);
        return _context.Sellers.FromSqlRaw("EXEC sp_GetPasswordSellerAccountByPhone @sSellerPhone", phoneParam);
    }

    public IEnumerable<Seller> getSellerAccountByID(int sellerID)
    {
        SqlParameter sellerIDParam = new SqlParameter("@PK_iSellerID", sellerID);
        return _context.Sellers.FromSqlRaw("EXEC sp_GetSellerAccountByID @PK_iSellerID", sellerIDParam);
    }

    public IEnumerable<SellerInfo> getSellerInfoByPhone(string phone)
    {
        SqlParameter phoneParam = new SqlParameter("@sSellerPhone", phone);
        return _context.SellerInfos.FromSqlRaw("EXEC sp_GetSellerInfoByPhone @sSellerPhone", phoneParam);
    }

    public IEnumerable<SellerInfo> getSellerInfoByPhoneAndPassword(string phone, string password)
    {
        password = encrypt(password);
        SqlParameter phoneParam = new SqlParameter("@sSellerPhone", phone);
        SqlParameter passwordParam = new SqlParameter("@sSellerPassword", password);
        return _context.SellerInfos.FromSqlRaw("EXEC sp_GetSellerInfoByPhoneAndPassword @sSellerPhone, @sSellerPassword", phoneParam, passwordParam);
    }

    public IEnumerable<SellerInfo> getSellerInfoBySellerID(int sellerID)
    {
        SqlParameter sellerIDParam = new SqlParameter("@PK_iSellerID", sellerID);
        return _context.SellerInfos.FromSqlRaw("EXEC sp_GetSellerInfoBySellerID @PK_iSellerID", sellerIDParam);
    }

    public IEnumerable<SellerInfo> getSellerInfoByShippingOrderID(int shippingOrderID)
    {
        SqlParameter shippingOrderIDParam = new SqlParameter("@PK_iShippingOrderID", shippingOrderID);
        return _context.SellerInfos.FromSqlRaw("EXEC sp_GetSellerInfoByShippingOrderID @PK_iShippingOrderID", shippingOrderIDParam);
    }

    public IEnumerable<Seller> loginAccount(string phone, string password)
    {
        SqlParameter phoneParam = new SqlParameter("@sSellerPhone", phone);
        SqlParameter passwordParam = new SqlParameter("@sSellerPassword", password);
        return _context.Sellers.FromSqlRaw("EXEC sp_LoginAccountSeller @sSellerPhone, @sSellerPassword", phoneParam, passwordParam);
    }

    public bool registerAccountSeller(string phone, string username, string password)
    {
        SqlParameter phoneParam = new SqlParameter("@sSellerPhone", phone);
        SqlParameter usernameParam = new SqlParameter("@sSellerUsername", username);
        SqlParameter passwordParam = new SqlParameter("@sSellerPassword", password);
        _context.Database.ExecuteSqlRaw("EXEC sp_RegisterAccountSeller @sSellerPhone, @sSellerUsername, @sSellerPassword", phoneParam, usernameParam, passwordParam);
        return true;
    }
}