//This is for the Product View Page

function ValidateItem(ele) {
	let qty = parseInt(ele.value);
	if (isNaN(qty) || qty <= 0) {
		ele.value = ele.defaultValue;
		ele.style.borderColor = "red";
		return false;
	} else {
		ele.style.borderColor = "black";
		ele.defaultValue = ele.value;
		return true;
	}
}

function ChangeItem(strId, isAdd) {
	let ele = document.getElementById(strId);
	if (ele == null) {
		return;
	}
	ele.style.borderColor = "black";
	let qty = parseInt(ele.value);
	if (isAdd) {
		qty += 1;
		ele.value = qty;
		ele.defaultValue = ele.value;
	} else {
		qty -= 1;
		if (qty > 0) {
			ele.value = qty
			ele.defaultValue = ele.value;
		}
	}
}

function ValidateAdd() {
	var eleqty = document.getElementById("Quantity");
	var prdId = document.getElementById("ProdId").value;
	if (ValidateItem(eleqty)) {
		AddToCart(prdId, eleqty.value);
	}
}

 