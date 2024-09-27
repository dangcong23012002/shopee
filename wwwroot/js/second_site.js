function getAPISite() {
    var xhr = new XMLHttpRequest();
    xhr.open('post', '/cart', true);
    xhr.onreadystatechange = () => {
        if (xhr.readyState == 4 && xhr.status == 200) {
            const data = JSON.parse(xhr.responseText);

            console.log(data);

            setAccount(data);
            
        }
    };
    xhr.send(null);
}
getAPISite();

function setAccount(data) {
    let htmlAccount = "";
    if (data.userID == 0) {
        htmlAccount += 
        `
                            <div class="header__item header__item-line">
                                <a class="header__item-link" href="/user/register">Đăng ký</a>
                            </div>
                            <div class="header__item header__item--has-user">
                                <a class="header__item-link" href="/user/login">Đăng nhập</a>
                            </div>                  
        `;
    } else {
        if (data.roleID == 2) {
            htmlAccount +=
            `
                            <div class="header__item header__item--has-user">
                                <a href="#" class="header__item-link">
                                    <img src="/img/no_user.jpg" class="header__item-img" alt="">
                                    <div class="header__item-sub">${data.username}</div>
                                </a>
                                <div class="header__item-user">
                                    <ul class="header__navbar-user-menu">
                                        <li class="header__navbar-user-item">
                                            <a href="/user/profile">Tài khoản của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="#">Địa chỉ của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/user/purchase">Đơn mua</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/admin">Kênh quản trị</a>
                                        </li>
                                        <li class="header__navbar-user-item header__navbar-user-item--separate">
                                            <a href="/user/logout">Đăng xuất</a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
            `;
        } else if (data.roleID == 3) {
            htmlAccount += 
            `
                            <div class="header__item header__item--has-user">
                                <a href="#" class="header__item-link">
                                    <img src="/img/no_user.jpg" class="header__item-img" alt="">
                                    <div class="header__item-sub">${data.username}</div>
                                </a>
                                <div class="header__item-user">
                                    <ul class="header__navbar-user-menu">
                                        <li class="header__navbar-user-item">
                                            <a href="/user/profile">Tài khoản của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="#">Địa chỉ của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/user/purchase">Đơn mua</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/picker">Kênh lấy hàng</a>
                                        </li>
                                        <li class="header__navbar-user-item header__navbar-user-item--separate">
                                            <a href="/user/logout">Đăng xuất</a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
            `;
        } else if (data.roleID == 4) {
            htmlAccount += 
            `
                            <div class="header__item header__item--has-user">
                                <a href="#" class="header__item-link">
                                    <img src="/img/no_user.jpg" class="header__item-img" alt="">
                                    <div class="header__item-sub">${data.username}</div>
                                </a>
                                <div class="header__item-user">
                                    <ul class="header__navbar-user-menu">
                                        <li class="header__navbar-user-item">
                                            <a href="/user/profile">Tài khoản của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="#">Địa chỉ của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/user/purchase">Đơn mua</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/delivery">Kênh giao hàng</a>
                                        </li>
                                        <li class="header__navbar-user-item header__navbar-user-item--separate">
                                            <a href="/user/logout">Đăng xuất</a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
            `;
        } else {
            htmlAccount += 
            `
                            <div class="header__item header__item--has-user">
                                <a href="#" class="header__item-link">
                                    <img src="/img/no_user.jpg" class="header__item-img" alt="">
                                    <div class="header__item-sub">${data.username}</div>
                                </a>
                                <div class="header__item-user">
                                    <ul class="header__navbar-user-menu">
                                        <li class="header__navbar-user-item">
                                            <a href="/user/profile">Tài khoản của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="#">Địa chỉ của tôi</a>
                                        </li>
                                        <li class="header__navbar-user-item">
                                            <a href="/user/purchase">Đơn mua</a>
                                        </li>
                                        <li class="header__navbar-user-item header__navbar-user-item--separate">
                                            <a href="/user/logout">Đăng xuất</a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
            `;
        }
    }
    document.querySelector(".header__item-auth").innerHTML = htmlAccount;
}

// Toast
function toast({ title = "", msg = "", type = "", duration = 3000}) {
    const main = document.getElementById('toast');
    if (main) {
        const toast = document.createElement("div");
        const autoRemoveId = setTimeout(() => {
            main.removeChild(toast);
        }, duration + 1000);

        toast.onclick = (e) => {
            if (e.target.closest('.toast__close')) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        };

        const icons = {
            success: 'uil uil-check-circle',
            error: 'uil uil-exclamation-triangle'
        };

        icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        toast.classList.add('toast', `toast--${type}`);
        toast.style.animation = `slideInLeft ease .3s, fadeOut linear 1s ${delay}s forwards`;
        toast.innerHTML = `
            <div class="toast__icon">
                <i class="${icon}"></i>
            </div>
            <div class="toast__body">
                <h3 class="toast__title">${title}</h3>
                <p class="toast__msg">${msg}</p>
            </div>
            <div class="toast__close">
                <i class="uil uil-times"></i>
            </div>
        `;
        main.appendChild(toast);
    }
}

// Tách lấy chữ số
// Nguồn: http://vncoding.net/2015/10/30/tach-cac-chu-so-thuoc-hang-tram-hang-chuc-hang-don-vi/
function money(number) {
    let result = ``; // Nếu là chuỗi thì trán đặt biến là const
    // Vì Const là một hằng số, vì vậy khi khai báo biến const thì bạn phải gán giá trị cho nó luôn, 
    // sau này cũng không thể thay đổi giá trị cho biến.
    // Nguồn: https://freetuts.net/bien-va-khai-bao-bien-trong-javascript-265.html
    // Ví dụ số 9899999
    let millions = Math.floor(number / 1000000); // Chia cho 1000000 và làm tròn số ta được 9
    let hundred_thousand = Math.floor((number % 1000000) / 100000); // Chia lấy phần dư ta được 899999 và tiếp tục chia cho 100000 và làm tròn ta được 8
    let tens_of_thousands = Math.floor((number % 1000000 % 100000) / 10000); // Tương tự lấy phần dư của hàng trục nghìn rồi chia cho 10000 ta được 9
    let thousand = Math.floor((number % 1000000 % 100000 % 10000) / 1000); // Lấy phần dư hàng nghìn rồi chia cho 1000
    let hundreds = Math.floor((number % 1000000 % 100000 % 10000 % 1000) / 100); // Lấy phần dư hàng trăm chia cho 100
    let tens = Math.floor((number % 1000000 % 100000 % 10000 % 1000 % 100) / 10); // Lấy phần dư của hàng chục chia cho 10
    let unit = Math.floor(number % 1000000 % 100000 % 10000 % 1000 % 100 % 10); // Lấy phần dư hàng đơn vị
    if (millions == 0 && hundred_thousand != 0) {
        result = `${hundred_thousand}${tens_of_thousands}${thousand}.${hundreds}${tens}${unit}`;
    } else if (millions == 0 && hundred_thousand == 0) {
        result = `${tens_of_thousands}${thousand}.${hundreds}${tens}${unit}`;
    } else {
        result = `${millions}.${hundred_thousand}${tens_of_thousands}${thousand}.${hundreds}${tens}${unit}`;
    }
    return result;
}