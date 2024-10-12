
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>


$(document).ready(function() {
    // Fade-in effect for the logo
    $('.logo img').hide().fadeIn(1000);

    // Dropdown animation
    $('.dropdown-toggle').on('click', function() {
        $(this).next('.dropdown-menu').slideToggle();
    });

    // Close dropdown when clicking outside
    $(document).on('click', function(e) {
        if (!$(e.target).closest('.btn-group').length) {
            $('.dropdown-menu').slideUp();
        }
    });

    // Smooth scrolling for links (if applicable)
    $('a[href*="#"]').on('click', function(event) {
        event.preventDefault();
        $('html, body').animate({
            scrollTop: $($(this).attr('href')).offset().top
        }, 500);
    });
});

