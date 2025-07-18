function CreatePagination(totalItems, currentPage, itemsPerPage, paginationId) {
	const $pagination = paginationId;
	$pagination.empty();

	if (totalItems === 0) {
		return;
	}

	const totalPages = Math.ceil(totalItems / itemsPerPage);

	// Nút Previous
	const $prevLi = $("<li>").addClass("page-item " + (currentPage === 1 ? "disabled" : ""));
	const $prevLink = $("<a>")
		.addClass("page-link")
		.attr("href", "#")
		.attr("aria-label", "Previous")
		.append("Trang trước");
	$prevLi.append($prevLink);
	$pagination.append($prevLi);

	// Xác định phạm vi trang hiển thị
	let startPage = Math.max(1, currentPage - 2);
	const endPage = Math.min(totalPages, startPage + 4);
	if (endPage - startPage < 4) {
		startPage = Math.max(1, endPage - 4);
	}

	// Nút trang đầu
	if (startPage > 1) {
		const $firstLi = $("<li>").addClass("page-item");
		const $firstLink = $("<a>").addClass("page-link").attr("href", "#").attr("data-page", "1").text("1");
		$firstLi.append($firstLink);
		$pagination.append($firstLi);

		if (startPage > 2) {
			const $ellipsisLi = $("<li>").addClass("page-item disabled");
			const $ellipsisLink = $("<a>").addClass("page-link").attr("href", "#").text("...");
			$ellipsisLi.append($ellipsisLink);
			$pagination.append($ellipsisLi);
		}
	}

	// Các nút trang
	for (let i = startPage; i <= endPage; i++) {
		const $li = $("<li>").addClass("page-item " + (i === currentPage ? "active" : ""));
		const $link = $("<a>").addClass("page-link").attr("href", "#").attr("data-page", i).text(i);
		$li.append($link);
		$pagination.append($li);
	}

	// Nút trang cuối
	if (endPage < totalPages) {
		if (endPage < totalPages - 1) {
			const $ellipsisLi = $("<li>").addClass("page-item disabled");
			const $ellipsisLink = $("<a>").addClass("page-link").attr("href", "#").text("...");
			$ellipsisLi.append($ellipsisLink);
			$pagination.append($ellipsisLi);
		}

		const $lastLi = $("<li>").addClass("page-item");
		const $lastLink = $("<a>").addClass("page-link").attr("href", "#").attr("data-page", totalPages).text(totalPages);
		$lastLi.append($lastLink);
		$pagination.append($lastLi);
	}

	// Nút Next
	const $nextLi = $("<li>").addClass("page-item " + (currentPage === totalPages ? "disabled" : ""));
	const $nextLink = $("<a>")
		.addClass("page-link")
		.attr("href", "#")
		.attr("aria-label", "Next")
		.append("Trang sau");
	$nextLi.append($nextLink);
	$pagination.append($nextLi);

	// Thêm sự kiện click cho các nút phân trang
	$pagination.find(".page-link").on("click", function (e) {
		e.preventDefault();

		const $this = $(this);
		const $parent = $this.parent();

		// Bỏ qua nếu là disabled hoặc active
		if ($parent.hasClass("disabled") || $parent.hasClass("active")) {
			return;
		}

		if ($this.attr("aria-label") === "Previous" && currentPage > 1) {
			loadPage(currentPage - 1);
		} else if ($this.attr("aria-label") === "Next" && currentPage < totalPages) {
			loadPage(currentPage + 1);
		} else if ($this.data("page")) {
			loadPage(parseInt($this.data("page")));
		}
	});

	// Hiển thị thông tin trang
	const startItem = (currentPage - 1) * itemsPerPage + 1;
	const endItem = Math.min(currentPage * itemsPerPage, totalItems);

	// Cập nhật thông tin hiển thị (nếu có element để hiển thị)
	if ($("#pagination-info").length > 0) {
		$("#pagination-info").text(`Hiển thị ${startItem}-${endItem} trong tổng số ${totalItems} bản ghi`);
	}
}

function CreatePaginationMinimal(totalItems, currentPage, itemsPerPage, paginationId) {
	const $pagination = paginationId;
	$pagination.empty();

	if (totalItems === 0) {
		return;
	}

	const totalPages = Math.ceil(totalItems / itemsPerPage);

	// Nút Previous
	const $prevLi = $("<li>").addClass("page-item " + (currentPage === 1 ? "disabled" : ""));
	const $prevLink = $("<a>")
		.addClass("page-link")
		.attr("href", "#")
		.attr("aria-label", "Previous")
		.append("<").on("click", () => loadPage(currentPage - 1));;
	$prevLi.append($prevLink);
	$pagination.append($prevLi);

	// Hiển thị trang hiện tại và tổng số trang
	const $pageInfo = $("<li>")
		.addClass("page-item")
		.append(
			$("<span>").addClass("page-link small text-muted").text(`${currentPage} / ${totalPages}`)
		);
	$pagination.append($pageInfo);

	const $nextLi = $("<li>").addClass("page-item " + (currentPage === totalPages ? "disabled" : ""));
	const $nextLink = $("<a>")
		.addClass("page-link")
		.attr("href", "#")
		.attr("aria-label", "Next")
		.append(">").on("click", () => loadPage(currentPage + 1));
	$nextLi.append($nextLink);
	$pagination.append($nextLi);

	// Thông tin số lượng bản ghi
	if ($("#pagination-info").length > 0) {
		const startItem = (currentPage - 1) * itemsPerPage + 1;
		const endItem = Math.min(currentPage * itemsPerPage, totalItems);
		$("#pagination-info").text(`Hiển thị: ${startItem}-${endItem} / ${totalItems} bản ghi`);
	}
}


function formatDateTime(date) {
	const year = date.getFullYear();
	const month = String(date.getMonth() + 1).padStart(2, '0');
	const day = String(date.getDate()).padStart(2, '0');
	const hours = String(date.getHours()).padStart(2, '0');
	const minutes = String(date.getMinutes()).padStart(2, '0');
	const seconds = String(date.getSeconds()).padStart(2, '0');

	return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}