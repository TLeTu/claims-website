// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Small UX touches without affecting backend behavior
(function () {
	// Animate main content on load for a refined entry
	window.addEventListener('load', function () {
		document.body.classList.add('ready');
	});

	// Keyboard focus outlines visible only via keyboard navigation
	function handleFirstTab(e) {
		if (e.key === 'Tab') {
			document.documentElement.classList.add('user-is-tabbing');
			window.removeEventListener('keydown', handleFirstTab);
		}
	}
	window.addEventListener('keydown', handleFirstTab);
})();
