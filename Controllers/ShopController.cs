using Microsoft.AspNetCore.Mvc;
using Project.Models;

public class ShopController : Controller
{
    private readonly IHttpContextAccessor _accessor;
        private readonly IHomeResponsitory _homeResponsitory;
        private readonly IShopResponsitory _shopResponsitory;
        private readonly ICartReponsitory _cartResponsitory;
        private readonly IUserResponsitory _userResponsitory;
        private readonly IProductResponsitory _productResponsitory;
        private readonly IChatRepository _chatRepository;

        public ShopController(
            IHttpContextAccessor accessor, 
            IHomeResponsitory homeResponsitory, 
            ICartReponsitory cartReponsitory, 
            IUserResponsitory userResponsitory, 
            IShopResponsitory shopResponsitory,
            IProductResponsitory productResponsitory,
            IChatRepository chatRepository
            )
        {
            _accessor = accessor;
            _homeResponsitory = homeResponsitory;
            _cartResponsitory = cartReponsitory;
            _userResponsitory = userResponsitory;
            _shopResponsitory = shopResponsitory;
            _productResponsitory = productResponsitory;
            _chatRepository = chatRepository;
        }

    [HttpGet]
    [Route("/shop/{shopUsername?}")]
    public IActionResult Index(string shopUsername = "") {
        // Lấy Cookies trên trình duyệt
        var userID = Request.Cookies["UserID"];
        if (userID != null)
        {
            _accessor?.HttpContext?.Session.SetInt32("UserID", Convert.ToInt32(userID));
        }
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        System.Console.WriteLine("sessionUserID: " + sessionUserID);
        if (userID != null)
        {
            List<User> users = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();
            _accessor?.HttpContext?.Session.SetString("UserName", users[0].sUserName);
            _accessor?.HttpContext?.Session.SetInt32("RoleID", users[0].FK_iRoleID);
        }
        else
        {
            _accessor?.HttpContext?.Session.SetString("UserName", "");
        }
        List<Store> store = _shopResponsitory.getShopByUsername(shopUsername).ToList();
        _accessor?.HttpContext?.Session.SetInt32("CurrentShopID", store[0].PK_iStoreID);
        _accessor?.HttpContext?.Session.SetString("CurrentShopUsername", shopUsername);
        return View();
    }

    [HttpPost]
    [Route("/shop/get-data")]
    public IActionResult GetData(int currentPage = 1, int categoryID = 0) {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        var sessionUsername = _accessor?.HttpContext?.Session?.GetString("UserName");
        var sessionRoleID = _accessor?.HttpContext?.Session?.GetInt32("RoleID");
        var sessionCurrentShopUsername = _accessor?.HttpContext?.Session.GetString("CurrentShopUsername");
        var sessionCurrentShopID = _accessor?.HttpContext?.Session.GetInt32("CurrentShopID");
        var shop = _shopResponsitory.getShopByUsername(sessionCurrentShopUsername);
        IEnumerable<MakeFriend> makeFriends = _chatRepository.getMakeFriendByUserIDAndShopID(Convert.ToInt32(sessionUserID), Convert.ToInt32(sessionCurrentShopID));
        IEnumerable<Product> products;
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID));
        IEnumerable<SliderShop> slidersShop = _shopResponsitory.getSlidersShopByShopID(Convert.ToInt32(sessionCurrentShopID));
        IEnumerable<Category> categories = _shopResponsitory.getCategoriesByShopID(Convert.ToInt32(sessionCurrentShopID));
        if (categoryID != 0) {
            products = _productResponsitory.getProductsByCategoryID(Convert.ToInt32(categoryID));
        } else {
            products = _shopResponsitory.getProductsByShopID(Convert.ToInt32(sessionCurrentShopID));
        }
        IEnumerable<Product> top3SellingProducts = _shopResponsitory.getTop3SellingProductsShop(Convert.ToInt32(sessionCurrentShopID));
        IEnumerable<Product> top10SellingProducts = _shopResponsitory.getTop10SellingProductsShop(Convert.ToInt32(sessionCurrentShopID));
        IEnumerable<Product> top10GoodPriceProducts = _shopResponsitory.getTop10GoodPriceProductsShop(Convert.ToInt32(sessionCurrentShopID));
        IEnumerable<Product> top10SuggestProducts = _shopResponsitory.getTop10SuggestProductsShop(Convert.ToInt32(sessionCurrentShopID));
        IEnumerable<Chat> chats = _chatRepository.getChatByUserID(Convert.ToInt32(sessionUserID));
        int totalRecord = products.Count();
        int pageSize = 10;
        int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
        products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
        ShopViewModel model = new ShopViewModel {
            Stores = shop,
            MakeFriends = makeFriends,
            SlidersShop = slidersShop,
            Categories = categories,
            Products = products,
            Top3SellingProducts = top3SellingProducts,
            Top10SellingProducts = top10SellingProducts,
            Top10GoodPriceProducts = top10GoodPriceProducts,
            Top10SuggestProducts = top10SuggestProducts,
            TotalPage = totalPage,
            PageSize = pageSize,
            CurrentPage = currentPage,
            RoleID = Convert.ToInt32(sessionRoleID),
            UserID = Convert.ToInt32(sessionUserID),
            Username = sessionUsername,
            CartDetails = cartDetails,
            CartCount = cartDetails.Count(),
            CurrentCategoryID = categoryID,
            Chats = chats
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/shop/sort-price/{sortType?}")]
    public IActionResult SortPrice(string sortType = "", int currentPage = 1) {
        int shopID = Convert.ToInt32(_accessor?.HttpContext?.Session.GetInt32("CurrentShopID"));
        IEnumerable<Product> products;
        if (sortType == "asc") {
            products = _productResponsitory.getProductsByShopIDAndSortIncre(shopID);
        } else {
            products = _productResponsitory.getProductsByShopIDAndSortReduce(shopID);
        }
        int totalRecord = products.Count();
        int pageSize = 10;
        int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
        products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
        ShopViewModel model = new ShopViewModel {
            Products = products,
            TotalPage = totalPage,
            PageSize = pageSize,
            CurrentPage = currentPage
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/shop/send-make-friend")]
    public IActionResult SendFriend(int sellerID = 0) {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        if (sessionUserID == null) {
            sessionUserID = 0;
        } 
        List<User> user = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();
        Status status;
        if (user.Count() == 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Bạn phải đăng nhập thì mới kết bạn được!"
            };
        } else {
            _chatRepository.insertMakeFriend(Convert.ToInt32(sessionUserID), sellerID);
            status = new Status {
                StatusCode = 1,
                Message = "Gửi lời kết bạn thành công!"
            };
        }
        ShopeeViewModel model = new ShopeeViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/shop/acept-friend")]
    public IActionResult AceptFriend(int makeFriendID = 0, int sellerID = 0) {
        Status status;
        List<Store> store = _shopResponsitory.getShopBySellerID(sellerID).ToList();
        if (_chatRepository.updateMakeFriendAboutAcept(makeFriendID)) {
            string automatedChat = "Chào bạn, hiện tại bộ phận CSKH của " + store[0].sStoreName + " đã hết giờ làm việc." 
                                 + " Bạn vui lòng liên hệ vào khung giờ <b>8:00 - 17:00 (T2 - T6) & 8:00 - 14:00 Thứ 7</b> "
                                 + " hoặc để lại lời nhắn, chúng mình sẽ phản hồi ngay với bạn vào giờ làm việc kế tiếp."
                                 + " " + store[0].sStoreName + " Shop xin cảm ơn ❤️";
            _chatRepository.insertChat(makeFriendID, automatedChat);
            List<Chat> chat = _chatRepository.getChatByMakeFriendID(makeFriendID).ToList();
            _chatRepository.insertChatDetail(chat[0].PK_iChatID, sellerID, automatedChat);
            status = new Status {
                StatusCode = 1,
                Message = "Chấp nhận kết bạn thành công!"
            }; 
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Chấp nhận kết bạn thất bại!"
            };
        }
        ShopViewModel model = new ShopViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/shop/add-chat")]
    public IActionResult AddChat(int chatID = 0, string msg = "") {
        int personID = Convert.ToInt32(_accessor?.HttpContext?.Session.GetInt32("UserID"));
        Status status;
        if (_chatRepository.insertChatDetail(chatID, personID, msg)) {
            status = new Status {
                StatusCode = 1,
                Message = "Thêm trò chuyện thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Thêm trò chuyện thất bại!"
            };
        }
        DataViewModel model = new DataViewModel {
            Status = status
        };
        return Ok(model);
    }
}