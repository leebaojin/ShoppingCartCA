// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//-----------------------------Update items code for cart pages--------------------------------
function UpdateItem(elem) {
    //To search for cart item
    var eleId = elem.id;
    var splitId = eleId.split("-");
    if (splitId.length == 2) {
        var eletrigger = splitId[0];
        var rowId = splitId[1];
    } else {
        return;
    }
    var qtyele = document.getElementById("quantity-" + rowId);
    if (qtyele === null) {
        return;
    }
    if (isNaN(qtyele.value)) {
        qtyele.value = qtyele.defaultValue;
        return;
    }
    var newval = parseInt(qtyele.value,10);

    if (eletrigger === "minus") {
        newval--;
    } else if (eletrigger === "add") {
        newval++;
    } else if (eletrigger === "remove") {
        newval = 0;
    }
    SendCartItem(rowId, newval);
}



function SendCartItem(rowId, newval) {
    itemele = document.getElementById("cartno-" + rowId);
    if (itemele === null) {
        prdele = document.getElementById("prodno-" + rowId);
        if (prdele == null) {
            window.location.href = "../Cart";
            return;
        }
    }

    let xhr = new XMLHttpRequest();

    xhr.open("POST", "../Cart/UpdateCartItem");

    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status != 200) {
                return;
            }


            let data = JSON.parse(this.responseText);

            if (data.updateSuccess == false) {
                window.location.href = "../Cart";
                return;
            }
            document.getElementById("totalcost").innerHTML = data.totalprice;
            document.getElementById("totalcost2").innerHTML = data.totalprice;
            qtyele = document.getElementById("quantity-" + data.updateRow);

            if (data.removeItem != null) {
                parentrow = qtyele.closest(".cart-row");
                parentrow.parentNode.removeChild(parentrow);

            } else {
                document.getElementById("price-" + data.updateRow).innerHTML = data.price;
                qtyele.value = data.newqty;
                qtyele.defaultValue = data.newqty;

            }

        }
    }
    if (itemele == null) {
        let CartUpdateData = {
            "CartItemId": "0",
            "Newqty": newval,
            "UpdateRow": rowId,
            "ProdItemId": prdele.value
        };
        xhr.send(JSON.stringify(CartUpdateData));
    }
    else {
        let CartUpdateData = {
            "CartItemId": itemele.value,
            "Newqty": newval,
            "UpdateRow": rowId,
            "ProdItemId": "0"
        };
        xhr.send(JSON.stringify(CartUpdateData));
    }
}


function AddToCart(prodId,qtyIn) {
    if (prodId === null) {
        return;
    }
    if (isNaN(qtyIn)) {
        return;
    }
    let qty = parseInt(qtyIn,10);
    if (qty <= 0) {
        return;
    }

    let xhr = new XMLHttpRequest();

    xhr.open("POST", "../Cart/AddToCart");

    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8;");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status !== 200) {
                return;
            }

            let data = JSON.parse(this.responseText);

            if (data.addSuccess != true) {
                window.location.href = "../Logout";
                return;
            }

            cartele = document.getElementById("CartSize");
            if (cartele != null) {
                cartele.innerHTML = "Cart : " + data.cartqty;
                cartele.value = data.cartqty;
            }

            alert("Added to cart");
            return false;
        }
    }
    let DataCartProduct = {
        "ProdId": prodId,
        "Quantity": qty
    };

    xhr.send(JSON.stringify(DataCartProduct));
}

