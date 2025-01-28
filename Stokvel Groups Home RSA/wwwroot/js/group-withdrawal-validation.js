
//When the user selects "Bank Transfer" or "Mobile Payment", the "Transaction Reference" field will only accept numbers.
//If the user selects "Paypal", any string(alphanumeric or special characters) will be accepted in the "Transaction Reference" field.
$(document).ready(function () {
    // Function to check and apply validation on account number based on selected payment method
    $("#paymentMethod").change(function () {
        var paymentMethod = $(this).val();
        var accountNumberField = $("#accoutNumber");

        // Clear the previous validation
        accountNumberField.val('').removeAttr("pattern").removeAttr("title");

        // If Bank Transfer or Mobile Payment is selected, enforce numeric-only input
        if (paymentMethod === "Bank Transfer" || paymentMethod === "Mobile Payment") {
            accountNumberField.attr("pattern", "^[0-9]+$"); // Allow only numbers
            accountNumberField.attr("title", "Only numbers are allowed.");
        } else {
            // If Paypal is selected, you can allow a generic string (alphanumeric or special chars) for the transaction reference
            accountNumberField.removeAttr("pattern").removeAttr("title");
        }
    });

    $("#accoutNumber, #confirmAccountNumber").on("keyup", function () {
        var accountNumber = $("#accoutNumber").val();
        var confirmAccountNumber = $("#confirmAccountNumber").val();

        // Check if the two account numbers match
        if (accountNumber !== confirmAccountNumber) {
            $("#confirmError").show(); // Show error message
        } else {
            $("#confirmError").hide(); // Hide error message
        }
    });

    // Validate when the form is submitted
    $("form").submit(function (e) {
        var accountNumber = $("#accoutNumber").val();
        var confirmAccountNumber = $("#confirmAccountNumber").val();

        if (accountNumber !== confirmAccountNumber) {
            // Prevent form submission if they do not match
            e.preventDefault();
            alert("Account numbers do not match!");
        }
    });

    // Get references to the checkbox and submit button
    const confirmReminder = document.getElementById('confirmReminder');
    const submitBtn = document.getElementById('submitBtn');
    const checkboxError = document.getElementById('checkboxError');
    const createGroupForm = document.getElementById('createGroupForm');

    // Enable submit button only if checkbox is checked
    confirmReminder.addEventListener('change', function () {
        if (confirmReminder.checked) {
            submitBtn.disabled = false;
            checkboxError.style.display = 'none';
        } else {
            submitBtn.disabled = true;
            checkboxError.style.display = 'block';
        }
    });

    // Prevent form submission if checkbox is not checked
    createGroupForm.addEventListener('submit', function (event) {
        if (!confirmReminder.checked) {
            event.preventDefault();
            checkboxError.style.display = 'block';
        }
    });

    $(document).ready(function () {
        // Enable tooltips globally
        $('[data-toggle="tooltip"]').tooltip();
    });
});
