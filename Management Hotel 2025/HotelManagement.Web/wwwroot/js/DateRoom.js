document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('dateForm');
    const arrive = document.getElementById('arrive');
    const depart = document.getElementById('depart');
    const label = document.getElementById('selectedLabel');
    const splitScene = document.getElementById('splitScene');
    const split = splitScene.querySelector('.split');
    const splitCols = Array.from(split.querySelectorAll('.split-col'));
    const goBtn = document.getElementById('goBtn');
    const hero = document.querySelector('.hero');
    const overlay = document.querySelector('.overlay-card');
    const btnText = goBtn.querySelector('.btn-text');
    const btnSpinner = goBtn.querySelector('.btn-spinner');

    // helper: format date (YYYY-MM-DD -> dd/mm/yyyy)
    function formatDate(d) {
        if (!d) return '';
        const parts = d.split('-');
        if (parts.length !== 3) return d;
        return `${parts[2]}/${parts[1]}/${parts[0]}`;
    }

    function daysBetween(a, b) {
        const da = new Date(a);
        const db = new Date(b);
        const diff = Math.round((db - da) / (1000 * 60 * 60 * 24));
        return diff;
    }

    function setBusy(isBusy) {
        if (isBusy) {
            overlay.classList.add('busy');
            goBtn.disabled = true;
            btnSpinner.classList.remove('visually-hidden');
            btnText.classList.add('visually-hidden');
            arrive.disabled = true;
            depart.disabled = true;
        } else {
            overlay.classList.remove('busy');
            goBtn.disabled = false;
            btnSpinner.classList.add('visually-hidden');
            btnText.classList.remove('visually-hidden');
            arrive.disabled = false;
            depart.disabled = false;
        }
    }

    // set min dates: no past dates
    function setMinDates() {
        const today = new Date();
        const yyyy = today.getFullYear();
        const mm = String(today.getMonth() + 1).padStart(2, '0');
        const dd = String(today.getDate()).padStart(2, '0');
        const iso = `${yyyy}-${mm}-${dd}`;
        arrive.min = iso;
        if (!arrive.value) {
            arrive.value = '';
        }
        // ensure depart min is at least arrive or today
        depart.min = arrive.value ? arrive.value : iso;
    }

    // copy hero background to split cols to keep visual consistent
    function syncSplitBackground() {
        const style = window.getComputedStyle(hero);
        const bg = style.getPropertyValue('background-image');
        // fallback if none
        splitCols.forEach(col => {
            col.style.backgroundImage = bg || "url('/images/hotel.jpg')";
        });
    }

    // clear error state
    function clearError() {
        label.classList.remove('text-danger');
    }

    // show textual selection and nights
    function showSelection(a, d) {
        const nights = daysBetween(a, d);
        const nightsText = nights === 0 ? '0 đêm' : `${nights} ${nights === 1 ? 'đêm' : 'đêm'}`;
        label.classList.remove('text-danger');
        label.textContent = `Bạn đã chọn: đến ${formatDate(a)} — đi ${formatDate(d)} · ${nightsText}`;
    }

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const a = arrive.value;
        const d = depart.value;
        clearError();

        if (!a || !d) {
            label.textContent = 'Vui lòng chọn cả ngày đến và ngày đi.';
            label.classList.add('text-danger');
            return;
        }

        // validate: depart after or equal arrive
        const diff = daysBetween(a, d);
        if (diff < 0) {
            label.textContent = 'Ngày đi phải lớn hơn hoặc bằng ngày đến.';
            label.classList.add('text-danger');
            return;
        }

        showSelection(a, d);

        // visual busy state and flip
        setBusy(true);

        // ensure split background synced
        syncSplitBackground();

        // Small delay to let busy UI show, then start flip
        setTimeout(() => startFlipTransition(), 180);
    });

    function startFlipTransition() {
        // show split scene (keeps hero underneath)
        splitScene.classList.remove('d-none');
        splitScene.setAttribute('aria-hidden', 'false');

        // paint then flip
        requestAnimationFrame(() => {
            // small stagger: add flipping class
            split.classList.add('flipping');
        });

        // total duration: CSS duration + stagger offsets + small buffer
        const cssDuration = 700; // should match --transition-duration
        const totalDuration = cssDuration + (3 * 110) + 180;

        setTimeout(() => {
            // reveal overlay of split
            splitScene.classList.add('revealed');

            // allow some time to show revealed, then cleanup
            setTimeout(() => {
                split.classList.remove('flipping');
                // hide split visually but keep it ready for next time
                splitScene.classList.add('d-none');
                splitScene.setAttribute('aria-hidden', 'true');

                // restore UI (or navigate to results page here)
                setBusy(false);
            }, 380);
        }, totalDuration);
    }

    // Manage floating labels for inputs (because input[type=date] doesn't consistently trigger CSS placeholder rules)
    function updateFloatingFor(el) {
        const container = el.closest('.form-floating');
        if (!container) return;
        if (el.value && el.value.trim() !== '') {
            container.classList.add('floating');
        } else {
            container.classList.remove('floating');
        }
    }

    // pressing Enter on date fields to submit + keep floating updated
    [arrive, depart].forEach(function (el) {
        el.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                goBtn.click();
            }
        });

        el.addEventListener('input', function () {
            clearError();
            // keep depart.min synced
            if (el === arrive) {
                depart.min = arrive.value ? arrive.value : depart.min;
            }
            updateFloatingFor(el);
        });

        // init state on load
        updateFloatingFor(el);
    });

    // init
    setMinDates();
    syncSplitBackground();

    // keep min dates updated on load/focus (in case timezone changed)
    window.addEventListener('focus', setMinDates);
});