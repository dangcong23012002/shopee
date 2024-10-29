using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Project.Models;

[Route("/product")]
public class ProductController : Controller {
    private readonly IProductResponsitory _productResponsitory;
    private readonly IHttpContextAccessor _accessor;
    private readonly ICartReponsitory _cartResponsitory;
    private readonly IHomeResponsitory _homeResponsitory;
    private readonly IUserResponsitory _userResponsitory;
    private readonly IShopResponsitory _shopResponsitory;
    public ProductController(IProductResponsitory productResponsitory, ICartReponsitory cartReponsitoty, IHttpContextAccessor accessor, IHomeResponsitory homeResponsitory, IUserResponsitory userResponsitory, IShopResponsitory shopResponsitory)
    {
        _productResponsitory = productResponsitory;
        _cartResponsitory = cartReponsitoty;
        _accessor = accessor;
        _homeResponsitory = homeResponsitory;
        _userResponsitory = userResponsitory;
        _shopResponsitory = shopResponsitory;
    }

    [Route("{parentCategoryID}")]
    [HttpGet]
    public IActionResult Index(int parentCategoryID) {
        _accessor?.HttpContext?.Session.SetInt32("CurrentParentCategoryID", parentCategoryID);
        // Vì mình lấy layout của _Layout của kiểu là @model ProducdViewModel nó sẽ chung cho tất cả các trang, ta lấy riêng nó sẽ lỗi
        return View();
    }

    [HttpPost("get-data")]
    [Route("get-data")]
    public IActionResult GetData(int categoryID = 0, int currentPage = 1) {
        IEnumerable<Product> products;
        IEnumerable<Product> productsByCategoryID;
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        var sessionParentCategoryID = _accessor?.HttpContext?.Session.GetInt32("CurrentParentCategoryID");
        List<User> users = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();
        if (users.Count() == 0) {
            products = _productResponsitory.getProductsByParentCategoryID(Convert.ToInt32(sessionParentCategoryID));
        } else if (users[0].FK_iRoleID == 2) {
            products = _productResponsitory.getProductsByParentCategoryID(Convert.ToInt32(sessionParentCategoryID));
        } else {
            products = _productResponsitory.getProductsByParentCategoryID(Convert.ToInt32(sessionParentCategoryID));
        }
        if (categoryID != 0)
        {
            products = _productResponsitory.getProductsByCategoryID(Convert.ToInt32(categoryID));
        }
        int totalRecord = products.Count();
        int pageSize = 10;
        int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
        products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID)).ToList();
        IEnumerable<Store> stores = _shopResponsitory.getShopByParentCategoryID(Convert.ToInt32(sessionParentCategoryID));
        IEnumerable<Category> categories = _homeResponsitory.getCategoriesByParentCategoryID(Convert.ToInt32(sessionParentCategoryID));
        // Vì mình lấy layout của _Layout của kiểu là @model ProducdViewModel nó sẽ chung cho tất cả các trang, ta lấy riêng nó sẽ lỗi
        ShopeeViewModel model = new ShopeeViewModel {
            Products = products,
            Stores = stores,
            Categories = categories,
            CartDetails = cartDetails,
            CurrentCategoryID = categoryID,
            TotalPage = totalPage,
            PageSize = pageSize,
            CurrentPage = currentPage,
            CartCount = cartDetails.Count()
        };
        return Ok(model);
    }

    /// <summary>
    /// Nguồn: https://xuanthulab.net/asp-net-core-mvc-chi-tiet-ve-route-trong-asp-net-mvc.html
    /// </summary>
    [Route("detail/{id?}")]
    public IActionResult Detail(int id)
    {
        _accessor?.HttpContext?.Session.SetInt32("CurrentProductID", id);
        return View();
    }

    [HttpPost]
    [Route("/product/get-data-detail")]
    public IActionResult GetDetail() {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        var sessionCurrentProductID = _accessor?.HttpContext?.Session.GetInt32("CurrentProductID");
        var product = _productResponsitory.getProductByID(Convert.ToInt32(sessionCurrentProductID));
        List<Store> store = _shopResponsitory.getShopByProductID(Convert.ToInt32(sessionCurrentProductID)).ToList();
        IEnumerable<UserInfo> userInfos = _userResponsitory.checkUserInfoByUserID(Convert.ToInt32(sessionUserID));
        IEnumerable<Reviewer> reviewers = _productResponsitory.getReviewerByProductID(Convert.ToInt32(sessionCurrentProductID));
        Store storeDetail = new Store {
            PK_iStoreID = store[0].PK_iStoreID,
            sStoreName = store[0].sStoreName,
            sImageAvatar = store[0].sImageAvatar,
            sImageLogo = store[0].sImageLogo,
            sImageBackground = store[0].sImageBackground,
            sDesc = store[0].sDesc
        };
        ProductViewModel model = new ProductViewModel {
            Products = product,
            Store = storeDetail,
            UserInfos = userInfos,
            Reviewers = reviewers
        };
        return Ok(model);
    }

    [Route("sort/{categoryID?}/{sortType?}")]
    public IActionResult Sort(int categoryID, string sortType = "") {
        IEnumerable<Product> products;
        var userID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        if (sortType == "asc") {
            products = _productResponsitory.getProductsByCategoryIDAndSortIncre(categoryID); // Gọi đúng phương thức sắp xếp tăng dần nhé
        } else {
            products = _productResponsitory.getProductsByCategoryIDAndSortReduce(categoryID); // Gọi đúng phương thức sắp xếp giảm dần nhé
        }
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(userID));
        IEnumerable<Category> categories = _homeResponsitory.getCategories();
        ProductViewModel model = new ProductViewModel {
            Products = products,
            CartDetails = cartDetails,
            Categories = categories,
            CurrentCategoryID = categoryID
        };
        //return Json(model);
        return View("Index", model);
    }

    [HttpGet]
    [Route("/product/similar/{productSimilarID?}/{categorySimilar?}")]
    public IActionResult Similar(int productSimilarID, int categorySimilar)
    {
        _accessor?.HttpContext?.Session.SetInt32("ProductSimilarID", productSimilarID);
        _accessor?.HttpContext?.Session.SetInt32("CategorySimilarID", categorySimilar);
        ShopeeViewModel model = new ShopeeViewModel {
            
        };
        return View(model);
    }

    [HttpPost]
    [Route("/product/similar/get-data")]
    public IActionResult Similar(int currentPage = 1) {
        var sessionProductSimilarID = _accessor?.HttpContext?.Session.GetInt32("ProductSimilarID");
        var sessionCategorySimilarID = _accessor?.HttpContext?.Session.GetInt32("CategorySimilarID");
        List<Product> product = _productResponsitory.getProductByID(Convert.ToInt32(sessionProductSimilarID)).ToList();
        List<Store> store = _shopResponsitory.getShopByProductID(Convert.ToInt32(sessionProductSimilarID)).ToList();
        IEnumerable<Product> products = _productResponsitory.getProductsByCategoryID(Convert.ToInt32(sessionCategorySimilarID));
        Product productDetail = new Product {
            PK_iProductID = product[0].PK_iProductID,
            sProductName = product[0].sProductName,
            sImageUrl = product[0].sImageUrl,
            dPrice = product[0].dPrice,
            dPerDiscount = product[0].dPerDiscount
        };
        Store storeDetail = new Store {
            PK_iStoreID = store[0].PK_iStoreID,
            sStoreName = store[0].sStoreName,
            sImageAvatar = store[0].sImageAvatar,
            sImageLogo = store[0].sImageLogo,
            sImageBackground = store[0].sImageBackground,
            sDesc = store[0].sDesc
        };
        int totalRecord = products.Count();
        int pageSize = 6;
        int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
        products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
        ProductViewModel model = new ProductViewModel {
            Product = productDetail,
            Store = storeDetail,
            Products = products,
            TotalPage = totalPage,
            PageSize = pageSize,
            CurrentPage = currentPage
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/product/reviewer")]
    public IActionResult Reviewer(int productID = 0, string comment = "", int star = 0, string image = "") {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        Status status;
        if (_productResponsitory.insertProductReviewer(Convert.ToInt32(sessionUserID), productID, star, comment, image)) {
            status = new Status {
                StatusCode = 1,
                Message = "Thêm đánh giá thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Thêm đánh giá thất bại!"
            };
        }
        ProductViewModel model = new ProductViewModel {
            Status = status
        };
        return Ok(model);
    }
}