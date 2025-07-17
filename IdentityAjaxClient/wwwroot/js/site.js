// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// API URLs
const API_BASE = '/api/product'; // Local proxy controller
const AUTH_API_LOGIN = '/api/login'; // Local proxy controller for login
const AUTH_API_REGISTER = '/api/register'; // Local proxy controller for registration

// Ajax login to API
function loginAjax(email, password) {
    fetch(AUTH_API_LOGIN, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email: email, password: password })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        if (data.token) {
            // Save token to localStorage
            localStorage.setItem('jwtToken', data.token);
            localStorage.setItem('userEmail', data.user.email);
            localStorage.setItem('userId', data.user.id);
            localStorage.setItem('userRole', data.user.role);
            
            // Update UI
            updateAuthUI(true);
            alert('Login successful!');
            
            // Redirect to product page if on login page
            if (window.location.pathname.includes('/Login')) {
                window.location.href = '/';
            } else {
                // Refresh the current page to update UI based on new role
                location.reload();
            }
        } else {
            alert('Login failed: ' + (data.error || 'Unknown error'));
        }
    })
    .catch(error => {
        alert('Error: ' + error);
    });
}

// Register new user
function registerAjax(email, password, name) {
    fetch(AUTH_API_REGISTER, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ 
            email: email, 
            password: password,
            name: name,
            confirmPassword: password // For validation on the server side
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            alert('Registration successful! Please login.');
            // Clear the registration form
            document.getElementById('registerForm').reset();
            
            // Redirect to login page
            window.location.href = '/Login';
        } else {
            alert('Registration failed: ' + (data.error || 'Unknown error'));
        }
    })
    .catch(error => {
        alert('Error: ' + error);
    });
}

// Logout function
function logout() {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('userId');
    localStorage.removeItem('userRole');
    updateAuthUI(false);
    alert('Logged out successfully');
}

// Update UI based on authentication state and user role
function updateAuthUI(isLoggedIn) {
    const userEmail = localStorage.getItem('userEmail');
    const userRole = localStorage.getItem('userRole');
    const isAdmin = userRole === 'Admin';
    
    if (isLoggedIn) {
        // Show logged in section with role information
        $('#loginSection').hide();
        $('#userInfo').text(`Logged in as: ${userEmail} (${userRole})`);
        $('#loggedInSection').show();
        
        // Update navigation based on role
        if (isAdmin) {
            $('.admin-only').show();
        } else {
            $('.admin-only').hide();
        }
    } else {
        $('#loggedInSection').hide();
        $('#loginSection').show();
        $('.admin-only').hide();
    }
}

// Check if user is logged in on page load
$(document).ready(function() {
    const token = localStorage.getItem('jwtToken');
    if (token) {
        updateAuthUI(true);
    } else {
        updateAuthUI(false);
    }
});

// Login form submission
$('#loginForm').submit(function(e) {
    e.preventDefault();
    const email = $('#loginEmail').val();
    const password = $('#loginPassword').val();
    loginAjax(email, password);
});
