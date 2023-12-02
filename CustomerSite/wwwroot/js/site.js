const closeMenu = document.querySelector(".icon-close-menu");
const MenuItem = document.querySelector(".mobile-nav-container");
const openMenu = document.querySelector(".menu-icon");

closeMenu.addEventListener("click", () => {
  MenuItem.classList.toggle("close-menu");
});

openMenu.addEventListener("click", () => {
  MenuItem.classList.toggle("close-menu");
});