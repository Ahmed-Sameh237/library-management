(() => {
    const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // Navbar: add a shadow once the page is scrolled
    const nav = document.querySelector('.lib-navbar');
    const onScroll = () => nav && nav.classList.toggle('scrolled', window.scrollY > 8);
    onScroll();
    window.addEventListener('scroll', onScroll, { passive: true });

    // Dark / light toggle (the saved choice is applied in the layout's <head>)
    const toggle = document.getElementById('themeToggle');
    if (toggle) {
        toggle.addEventListener('click', () => {
            const root = document.documentElement;
            const next = root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
            root.setAttribute('data-bs-theme', next);
            try { localStorage.setItem('theme', next); } catch (e) { }
        });
    }

    // Reveal elements as they scroll into view
    const revealEls = document.querySelectorAll('.reveal');
    if ('IntersectionObserver' in window && !reduce) {
        const io = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('in');
                    io.unobserve(entry.target);
                }
            });
        }, { threshold: 0.15 });
        revealEls.forEach((el) => io.observe(el));
    } else {
        revealEls.forEach((el) => el.classList.add('in'));
    }

    // Count-up numbers
    const fmt = new Intl.NumberFormat();
    const countUp = (el) => {
        const target = Number(el.dataset.count) || 0;
        if (reduce || target === 0) { el.textContent = fmt.format(target); return; }
        const start = performance.now();
        const duration = 1200;
        const tick = (now) => {
            const p = Math.min((now - start) / duration, 1);
            const eased = 1 - Math.pow(1 - p, 3);          // ease-out
            el.textContent = fmt.format(Math.round(target * eased));
            if (p < 1) requestAnimationFrame(tick);
        };
        requestAnimationFrame(tick);
    };
    const counters = document.querySelectorAll('[data-count]');
    if ('IntersectionObserver' in window) {
        const co = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) { countUp(entry.target); co.unobserve(entry.target); }
            });
        }, { threshold: 0.6 });
        counters.forEach((el) => co.observe(el));
    } else {
        counters.forEach(countUp);
    }

    // Mouse-following spotlight on cards
    document.querySelectorAll('.spot').forEach((el) => {
        el.addEventListener('pointermove', (e) => {
            const r = el.getBoundingClientRect();
            el.style.setProperty('--mx', (e.clientX - r.left) + 'px');
            el.style.setProperty('--my', (e.clientY - r.top) + 'px');
        });
    });

    // Rotating headline word
    const rotator = document.querySelector('.rotator');
    if (rotator && !reduce) {
        const words = JSON.parse(rotator.dataset.words || '[]');
        let i = 0;
        if (words.length > 1) {
            setInterval(() => {
                rotator.classList.add('out');
                setTimeout(() => {
                    i = (i + 1) % words.length;
                    rotator.textContent = words[i];
                    rotator.classList.remove('out');
                }, 300);
            }, 2600);
        }
    }
})();