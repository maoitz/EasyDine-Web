// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// -- Mobile menu --

document.addEventListener('DOMContentLoaded', function () {
    // Find the header and its controls
    const header = document.querySelector('header.site-header');
    if (!header) return;

    const btn = header.querySelector('.hamburger');
    const panel = header.querySelector('#mobileMenu');

    if (!btn || !panel) return;

    function setOpen(isOpen) {
        panel.setAttribute('data-open', String(isOpen));
        btn.setAttribute('aria-expanded', String(isOpen));
    }

    btn.addEventListener('click', function () {
        const open = panel.getAttribute('data-open') === 'true';
        setOpen(!open);
    });

    // Close on Escape
    header.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') setOpen(false);
    });
});

// -- Hero Image Carousel --
document.addEventListener('DOMContentLoaded', function () {
    const hero = document.querySelector('.header-hero');
    if (!hero) return;

    const slides = Array.from(hero.querySelectorAll('.hero-slide'));
    if (slides.length <= 1) return;

    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const slideMs = prefersReduced ? 8000 : 9000; // == --hero-slide-duration
    let idx = slides.findIndex(s => s.getAttribute('data-active') === 'true');
    if (idx < 0) idx = 0;

    function setActive(next) {
        slides.forEach((s, i) => {
            const active = i === next;
            s.setAttribute('data-active', String(active));
            s.setAttribute('aria-hidden', String(!active));
        });
        idx = next;
    }

    // Cross-fade exactly when the outgoing slide has returned to scale(1)
    setInterval(() => setActive((idx + 1) % slides.length), slideMs);
});

// -- Menu page aside navigation (Scrollspy) --
document.addEventListener('DOMContentLoaded', function () {
    const aside = document.querySelector('.menu-aside');
    const links = Array.from(document.querySelectorAll('.menu-nav-link'));
    if (!aside || links.length === 0) return; // not on the Menu page

    const sections = links
        .map(a => {
            const id = (a.getAttribute('href') || '').replace('#','');
            const el = document.getElementById(id);
            return el ? { id, el, link: a } : null;
        })
        .filter(Boolean);

    if (sections.length === 0) return;

    function setActive(id){
        for (const s of sections) {
            s.link.setAttribute('aria-current', String(s.id === id));
        }
    }

    const observer = new IntersectionObserver((entries) => {
        const visible = entries
            .filter(e => e.isIntersecting)
            .sort((a,b) => Math.abs(a.boundingClientRect.top) - Math.abs(b.boundingClientRect.top));

        if (visible[0]) {
            const id = visible[0].target.id;
            setActive(id);
        }
    }, {
        root: null,
        rootMargin: '-64px 0px -66% 0px', // adjust if header height changes
        threshold: [0, 0.25, 0.5, 0.75, 1]
    });

    sections.forEach(s => observer.observe(s.el));

    // sync on click + back/forward
    links.forEach(a => {
        a.addEventListener('click', () => {
            const id = (a.getAttribute('href') || '').replace('#','');
            setActive(id);
        });
    });
    window.addEventListener('hashchange', () => {
        const id = location.hash.replace('#','');
        if (id) setActive(id);
    });

    // initialize
    const initial = location.hash.replace('#','') || sections[0].id;
    setActive(initial);
});



