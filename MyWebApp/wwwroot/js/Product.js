var pTable;
$(document).ready(function () {
    pTable = $("#productTable").DataTable({
            ajax: {
                url : "/Admin/Product/GetData"
            },
            columns: [
                { data: 'name' },
                { data: 'description' },
                { data: 'price' },
                { data: 'category.name' },
                {
                    data: "id",
                    render: function (data) {
                        return `
           <a href = "/Admin/Product/AddEditProduct?id=${data}"><i class="bi bi-pencil-square"></i></a>
                    <a asp-action="DeleteCategory" asp-controller="Category" asp-route-id="@item.Id"><i class="bi bi-trash-fill"></i></a>`;
                    }
                }
            ]
    });
});