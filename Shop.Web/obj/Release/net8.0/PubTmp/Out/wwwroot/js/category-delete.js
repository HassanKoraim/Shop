function deleteProduct(urlPath) {
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
                    url: urlPath,
                    type: "Delete",
                    success: function (data) {
                        if (data.success) {
                           // toastr.success(data.message);
                            Swal.fire("Deleted!", data.message, "success").then(() => {
                                location.reload(); // ✅ Reload page after alert is closed
                            });
                        } else {
                            toaster.error(data.message);
                        }
                    }
                });
                Swal.fire({
                    title: "Deleted!",
                    text: "Your Category has been deleted.",
                    icon: "success"
                });
            }
        });
    }
