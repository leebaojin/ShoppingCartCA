//This is for the Product View Page


function StarSelect(num) {
	if (num < -1 || num >= 5) {
		return;
	}
	if (document.getElementById("commentId").getAttribute("editstate") != "1") {
		return;
	}
	let notratedEle = document.getElementById("notrated");
	if (notratedEle != null) {
		if (num == -1) {
			notratedEle.style.display = "inline-block";
		} else {
			notratedEle.style.display = "none";
		}
	}
	for (let i = 0; i < 5; i++) {
		if (i <= num) {
			document.getElementById("ratestar-" + i).className = "fa fa-star checked";
		}
		else {
			document.getElementById("ratestar-" + i).className = "fa fa-star";
		}
	}
}

function GetStarRating() {
	var count = -1;
	for (let i = 0; i < 5; i++) {
		if (document.getElementById("ratestar-" + i).className == "fa fa-star checked") {
			count++;
		} else {
			break;
		}
	}
	return count;
}

function EditReview() {
	let btn = document.getElementById("review-editpost");
	btn.onclick = PostReview;
	btn.innerHTML = "Post";
	let refEle = document.getElementById("commentId");
	document.getElementById("review-body-temp").innerHTML = document.getElementById("review-body").innerHTML;
	document.getElementById("review-body").style.display = "none";
	document.getElementById("review-body-temp").style.display = "block";
	refEle.setAttribute("editstate", "1");
	document.getElementById("review-cancel").style.display = "inline-block";

}

function CancelEdit() {
	let btn = document.getElementById("review-editpost");
	let refEle = document.getElementById("commentId");
	StarSelect(refEle.getAttribute("starrating"));
	if (refEle.value == "null") {
		document.getElementById("review-body-temp").innerHTML = "";
		document.getElementById("review-body-temp").value = "";
		let notratedEle = document.getElementById("notrated");
		if (notratedEle != null) {
			notratedEle.style.display = "inline-block";
		}
	} else {
		document.getElementById("review-body-temp").innerHTML = document.getElementById("review-body").innerHTML;
		document.getElementById("review-body-temp").value = document.getElementById("review-body").innerHTML;
		document.getElementById("review-body-temp").style.display = "none"
		document.getElementById("review-body").style.display = "block";
		document.getElementById("review-cancel").style.display = "none";
		btn.onclick = EditReview;
		btn.innerHTML = "Edit";
		refEle.setAttribute("editstate", "0");
	}
}

function PostReview() {
	try {
		var reviewRating = GetStarRating();
		var prdId = document.getElementById("ProdId").value;
		var reviewMsg = document.getElementById("review-body-temp").value.trim();
		var commentId = document.getElementById("commentId").value;
	}
	catch (err) {
		alert("Failed to post review");
		window.location.reload();
		return;
	}

	if (isNaN(reviewRating)) {
		alert("Failed to post review");
		window.location.reload();
		return;
	}

	if (reviewRating >= 0 && reviewRating <= 4) {
		reviewRating += 1; //The starrating holds the index and not the actual rating. Actual rating needs to +1
	} else {
		reviewRating = -1;
	}

	let xhr = new XMLHttpRequest();

	xhr.open("POST", "/Home/SendReview")
	xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");

	xhr.onreadystatechange = function () {
		if (this.readyState === XMLHttpRequest.DONE) {
			if (this.status !== 200) {
				return;
			}

			let data = JSON.parse(this.responseText);

			if (data.postSuccess === false) {
				alert("Failed to post review");
				window.location.reload();
				return false;
			}

			let revbody = document.getElementById("review-body");
			let tempRevbody = document.getElementById("review-body-temp");
			let btn = document.getElementById("review-editpost");
			let refEle = document.getElementById("commentId");
			let reviewHead = document.getElementById("review-prompt");

			//set the star rating 
			StarSelect(data.starRating - 1); //Do note that the index of the star is -1 from the rating

			refEle.setAttribute("starrating", data.starRating); //Change the starrating
			revbody.innerHTML = tempRevbody.innerHTML; //Change the body
			revbody.value = tempRevbody.value;
			tempRevbody.style.display = "none" //display changes
			revbody.style.display = "block";
			document.getElementById("review-cancel").style.display = "none";
			btn.onclick = EditReview;
			btn.innerHTML = "Edit";
			refEle.setAttribute("editstate", "0"); //Change the editstate
			if (reviewHead != null) {
				reviewHead.innerHTML = "Your Review";
			}

			setTimeout(function () {
				alert("Post Successful");
				return;
			}, 0)
			return;
		}
	}
	let data = {
		"ReviewRating": reviewRating,
		"ReviewMsg": reviewMsg,
		"CommentId": commentId,
		"PrdId": prdId
	};

	xhr.send(JSON.stringify(data));
}
