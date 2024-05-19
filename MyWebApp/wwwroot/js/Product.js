var pTable;
$(document).ready(function () {
    pTable = $("#productTable").DataTable({
        ajax: {
            url: "/Admin/Product/GetData"
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
                    <a href='#' onClick=RemoveProduct("/Admin/Product/Delete?id=${data}") style='cursor:pointer;'><i class="bi bi-trash-fill"></i></a>`;
                }
            }
        ]
    });

});
function RemoveProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'Delete',
                success: function (data) {
                    if (data.success) {
                        pTable.ajax.reload();
                        toastr.success(data.messege);
                    } else {
                        toastr.Error(data.messege);
                    }
                }
            })
        }
    });
}