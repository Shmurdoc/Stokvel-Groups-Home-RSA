// Function to compare both select elements and show/hide warning
document.getElementById('roleSelect1').addEventListener('change', checkRoleMatch);
document.getElementById('roleSelect2').addEventListener('change', checkRoleMatch);
document.getElementById('registerSubmit').disabled = true; // disabled button true
function checkRoleMatch() {
    var role1 = document.getElementById('roleSelect1').value;
    var role2 = document.getElementById('roleSelect2').value;

    

    // Check if both values are the same
    if (role1 !== role2) {
        document.getElementById('roleWarning').style.display = 'block'; // Show warning
        document.getElementById('registerSubmit').disabled = true; // disabled button true
    } else {
        document.getElementById('roleWarning').style.display = 'none'; // Hide warning
        document.getElementById('registerSubmit').disabled = false; // disabled button false
    }
}