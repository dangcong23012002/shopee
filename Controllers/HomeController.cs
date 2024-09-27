﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Diagnostics;
using RouteAtrribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly IHomeResponsitory _homeResponsitory;
        private readonly ICartReponsitory _cartResponsitory;
        private readonly IUserResponsitory _userResponsitory;

        public HomeController(ILogger<HomeController> logger, DatabaseContext context, IHttpContextAccessor accessor, IHomeResponsitory homeResponsitory, ICartReponsitory cartReponsitory, IUserResponsitory userResponsitory)
        {
            _logger = logger;
            _context = context;
            _accessor = accessor;
            _homeResponsitory = homeResponsitory;
            _cartResponsitory = cartReponsitory;
            _userResponsitory = userResponsitory;
        }

        /// <summary>
        /// Điểm tích cự và hạn chế trong cách làm việc nhóm
        /// - Tích cực:
        ///     + Các thành viên trong nhóm có thể giúp đỡ nhau nếu, fix lỗi cho nhau nếu thành viên nào đó không fix dc lỗi 
        ///     + Các nhánh Git của từng thành viên dev đc phân chia rõ ràng để khi ghép code tranh được các xung đột về nhánh không đáng có
        ///     + Trưởng nhóm Dev luôn theo sát quá trình push của từng thành viên: khi có chức năng mới cần đước ghép vào nhánh chính sẽ họp (teamview) để thống nhất và gộp
        /// - Hạn chế:
        ///     + Các thành viên cũng chưa tích cực làm việc trên các nhánh mà mình được giao
        ///     + Khuyết một số chức năng, hoặc để ghép chúng một cách thống nhất vẫn là một câu hỏi
        /// - Khắc phục:
        ///     + Các thành viên cần tích cực hơn, có trách nhiệp hơn với nhánh của mình cũng như nhiệm vụ phải hoàn thiện
        ///     + Trưởng nhóm Dev phải là người thấu hiểu năng lực của các thành viên để phân bố công việc cho phù hợp cho từngg người
        ///     + Có vấn đề hay gặp bug thành viên dev phải luôn đưa ra để nhóm cùng tìm cách giải quyết
        /// </summary>
        /// <param name="currentPage"></param>
        /// <returns></returns>

        public IActionResult Index(int currentPage = 1)
        {
            // Lấy Cookies trên trình duyệt
            var userID = Request.Cookies["UserID"];
            if (userID != null) {
                _accessor?.HttpContext?.Session.SetInt32("UserID", Convert.ToInt32(userID));
            }
            var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
            if (sessionUserID == null) {
                _accessor?.HttpContext?.Session.SetInt32("UserID", 0);
            }
            System.Console.WriteLine("sessionUserID: " + sessionUserID);
            if (userID != null) {
                List<User> users = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();
                _accessor?.HttpContext?.Session.SetString("UserName", users[0].sUserName);
                _accessor?.HttpContext?.Session.SetInt32("RoleID", users[0].FK_iRoleID);
            } else {
                _accessor?.HttpContext?.Session.SetString("UserName", "");
            }
            return View();
        }

        [HttpPost]
        [Route("/home/get-data")]
        public IActionResult GetData(int currentPage = 1) {
            var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
            var sessionRoleID = _accessor?.HttpContext?.Session.GetInt32("RoleID");
            var sessionUsername = _accessor?.HttpContext?.Session.GetString("UserName");
            IEnumerable<Product> products = _homeResponsitory.getProducts().ToList();
            int totalRecord = products.Count();
            int pageSize = 12;
            int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
            IEnumerable<Store> stores = _homeResponsitory.getStores();
            IEnumerable<ParentCategory> parentCategories = _homeResponsitory.getParentCategories();
            IEnumerable<Category> categories = _homeResponsitory.getCategories().ToList();
            IEnumerable<Favorite> favorites = _homeResponsitory.getFavorites(Convert.ToInt32(sessionUserID));
            IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID)).ToList();
            IEnumerable<CartDetail> carts = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID));
            int cartCount = carts.Count();
            ShopeeViewModel model = new ShopeeViewModel {
                Stores = stores,
                Products = products,
                ParentCategories = parentCategories,
                Categories = categories,
                Favorites = favorites,
                CartDetails = cartDetails,
                TotalPage = totalPage,
                PageSize = pageSize,
                CurrentPage = currentPage,
                RoleID = Convert.ToInt32(sessionRoleID),
                UserID = Convert.ToInt32(sessionUserID),
                Username = sessionUsername,
                CartCount = cartCount
            };
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Search(string keyword = "") {
            IEnumerable<Category> products = _homeResponsitory.searchProductsByKeyword(keyword).ToList();
            return Ok(products);
        }

        public IActionResult Privacy()
        {
            _accessor?.HttpContext?.Session.SetString("StudentName", "Công");
            _accessor?.HttpContext?.Session.SetInt32("StudentID", 1);
            return View();
        }

        [HttpGet]
        [Route("/home/suggest")]
        public IActionResult Suggest() {
            var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
            ShopeeViewModel model = new ShopeeViewModel {
                RoleID = Convert.ToInt32(sessionUserID)
            };
            return View(model);  
        }

        [HttpPost]
        [Route("/home/suggest")]
        public IActionResult GetDataSuggest(int currentPage = 1) {
            IEnumerable<Product> products = _homeResponsitory.getProducts().ToList();
            int totalRecord = products.Count();
            int pageSize = 12;
            int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
            ShopeeViewModel model = new ShopeeViewModel {
                Products = products,
                TotalPage = totalPage,
                PageSize = pageSize,
                CurrentPage = currentPage
            };
            return Ok(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}