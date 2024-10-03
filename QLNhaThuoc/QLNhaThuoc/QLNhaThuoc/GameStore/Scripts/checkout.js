const fullname = document.getElementById('fullname');
const email = document.getElementById('email');
const phone = document.getElementById('phone');
const address = document.getElementById('address');

const city = document.getElementById('province'); //thanh pho
const district = document.getElementById('district'); //quan/huyen
const ward = document.getElementById('village');//

fetch('/data.json')
    .then((response) => response.json())
    .then((data) => renderCity(data));
function renderCity(data) {
    for (var item of data) {

        city.options[city.options.length] = new Option(item.Name, item.Id);
    }


    city.onchange = () => {
        district.length = 1;


        if (city.value != '') {

            const result = data.filter((n) => n.Id === city.value);

            //     => result[0].Districts
            for (var item of result[0].Districts) {
                district.options[district.options.length] = new Option(
                    item.Name,
                    item.Id
                );
            }
        } else {

        }
    };

    district.onchange = () => {
        ward.length = 1;
        const result = data.filter((el) => el.Id === city.value);
        if (district.value != ' ') {
            // l?y d? li?u qu?n v? trong d? li?u qu?n ch? t?n ???ng
            const resultDistrict = result[0].Districts.filter(
                (el) => el.Id === district.value
            );

            for (var item of resultDistrict[0].Wards) {
                ward.options[ward.options.length] = new Option(item.Name, item.id);
            }
        }
    };
}

import validation from './validation.js';


$('#save_btn').click(() => {
    let checkEmpty = validation.checkRequired([fullname, email, phone, address]);
    let checkEmailInvalid = validation.checkEmail(email);
    let checkPhoneInvalid = validation.checkNumberPhone(phone);
    let checkAddressInvalid = validation.checkAddress([city, district, ward]);
    if (
        checkEmpty &&
        checkEmailInvalid &&
        checkPhoneInvalid &&
        !checkAddressInvalid
    ) {
        let thanhpho = $("#province option:selected").text()
        let quan = $("#village option:selected").text()
        let phuong = $("#district option:selected").text()
        let diachi = address.value + ', ' + quan + ', ' + phuong + ', ' + thanhpho
        let dienthoai = Number(phone.value)

        let data = {
            fullName: fullname.value,
            email: email.value,
            phone: dienthoai,
            address: diachi,

        }

        $.ajax({
            type: "POST",
            url: '/giohang/ThanhToan',
            data: data,
            dataType: "json",
            cache: false,
            success: function (res) {
                if (res == 'succes') {
                    alert("Đặt hàng thành công! Đơn hàng của bạn sẽ sớm được giao");
                    window.location = "/home/index";
                }
            }
        });
    }

})