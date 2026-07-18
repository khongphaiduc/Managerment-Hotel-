// intro.js — starfield + meteors (hoàn thiện, tăng khả năng lấp lánh của sao)
// Meteors: shooting stars spawn roughly every ~2s, hướng rơi quanh 7 giờ (down-left).
// Meteor spawner bắt đầu sau khi intro content được revealed to avoid meteors while curtains are closed.

document.addEventListener('DOMContentLoaded', function () {
    // ---------- Canvas / Starfield / Meteor state ----------
    const canvas = document.getElementById('starfield');
    const ctx = canvas && canvas.getContext ? canvas.getContext('2d') : null;
    const stars = [];
    const meteors = [];
    let animId = null;
    let visible = true;
    let spawnTimer = null;
    let spawnerStarted = false;

    // Helper: chuyển "giờ" sang độ (0=right, 90=down, 180=left, 270=up)
    function hourToDeg(hour) {
        // hour: 1..12. 3 -> 0°, 6 -> 90°, 9 -> 180°, 12 -> 270°
        let deg = ((hour - 3) * 30) % 360;
        if (deg < 0) deg += 360;
        return deg;
    }

    // Meteor configuration
    const meteorConfig = {
        spawnInterval: 2000,      // average ms between spawns
        spawnJitter: 600,         // +/- jitter ms
        maxConcurrent: 4,         // max meteors at once
        minSpeed: 900,            // px/s
        maxSpeed: 1600,
        minLength: 120,           // trail length px
        maxLength: 340,
        alpha: 0.98,
        spawnMargin: 40,
        // choose direction around 7 o'clock
        angleCenterDeg: hourToDeg(7), // ~120°
        angleSpreadDeg: 6,             // ±6°
        // chance that a spawn "burst" will contain 2 meteors instead of 1 (0..1)
        doubleSpawnChance: 0.42
    };

    // derived angle range
    meteorConfig.angleMinDeg = meteorConfig.angleCenterDeg - meteorConfig.angleSpreadDeg;
    meteorConfig.angleMaxDeg = meteorConfig.angleCenterDeg + meteorConfig.angleSpreadDeg;

    // ---------- Canvas resize & star creation ----------
    function resizeCanvas() {
        if (!canvas || !ctx) return;
        const dpr = window.devicePixelRatio || 1;
        const width = Math.max(300, window.innerWidth);
        const height = Math.max(200, window.innerHeight);
        canvas.width = Math.round(width * dpr);
        canvas.height = Math.round(height * dpr);
        canvas.style.width = width + 'px';
        canvas.style.height = height + 'px';
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);

        createStars(width, height);
    }

    function createStars(width, height) {
        stars.length = 0;
        const density = 0.00009;
        let count = Math.round(width * height * density);
        count = Math.max(60, Math.min(count, 420)); // clamp for perf

        for (let i = 0; i < count; i++) {
            // tune: larger variance for more visual depth
            const r = Math.random() * 1.6 + Math.random() * 1.6;
            const star = {
                x: Math.random() * width,
                y: Math.random() * height * 0.7 + (height * 0.15),
                r,
                // baseAlpha increased a bit so stars are more visible; some stars brighter
                baseAlpha: 0.45 + Math.random() * 0.6, // was 0.35..1.1 -> now 0.45..
                // twinkle speed increased and varied more
                twinkleSpeed: 0.9 + Math.random() * 2.2, // was 0.6..2.2
                phase: Math.random() * Math.PI * 2,
                // twinkleRange increased: more amplitude
                twinkleRange: 0.8 + Math.random() * 1.2, // was 0.5..1.4
                hueShift: (Math.random() - 0.5) * 18,
                // sparkle / pop parameters: occasional quick bright spikes
                // next spark time (seconds)
                sparkNext: (Math.random() * 6) + Math.random() * 6, // first spark in 0..12s
                sparkDuration: 0.12 + Math.random() * 0.6, // length of spark
                sparkStrength: 0.8 + Math.random() * 1.6, // additional alpha boost during spark
                // internal: last spark time (seconds), initially negative
                sparkLast: -999
            };
            stars.push(star);
        }
    }

    // ---------- Meteor creation helpers ----------
    function createMeteorInstance(overrides = {}) {
        if (!canvas) return null;

        const w = canvas.width / (window.devicePixelRatio || 1);
        const h = canvas.height / (window.devicePixelRatio || 1);

        const margin = meteorConfig.spawnMargin;
        // base spawn near top-right
        let startX = w - (Math.random() * (margin * 6));
        let startY = Math.random() * (margin * 6);

        if (Math.random() < 0.6) startX = w + Math.random() * 80;
        if (Math.random() < 0.35) startY = -Math.random() * 80;

        // allow overrides for offsetting second meteor in burst
        if (overrides.startX !== undefined) startX += overrides.startX;
        if (overrides.startY !== undefined) startY += overrides.startY;

        // pick angle in configured range (degrees -> radians), allow small override
        const degBase = meteorConfig.angleMinDeg + Math.random() * (meteorConfig.angleMaxDeg - meteorConfig.angleMinDeg);
        const deg = overrides.angleDeg !== undefined ? overrides.angleDeg : degBase;
        const angle = deg * Math.PI / 180;

        const speed = meteorConfig.minSpeed + Math.random() * (meteorConfig.maxSpeed - meteorConfig.minSpeed);
        const vx = Math.cos(angle) * speed;
        const vy = Math.sin(angle) * speed;

        const length = meteorConfig.minLength + Math.random() * (meteorConfig.maxLength - meteorConfig.minLength);
        const life = (Math.sqrt(w * w + h * h) / speed) + 0.4; // seconds

        const meteor = {
            x: startX,
            y: startY,
            vx,
            vy,
            length,
            created: performance.now() * 0.001,
            life,
            fadeStart: 0.85 + Math.random() * 0.12,
            brightness: 0.9 + Math.random() * 0.9,
            width: 1.2 + Math.random() * 2.2
        };

        return meteor;
    }

    // Spawn burst: 1 or 2 meteors depending on chance and available slots
    function spawnBurst() {
        if (!canvas) return;
        const availableSlots = Math.max(0, meteorConfig.maxConcurrent - meteors.length);
        if (availableSlots === 0) return;

        const wantDouble = Math.random() < meteorConfig.doubleSpawnChance;
        const targetCount = wantDouble ? 2 : 1;
        const count = Math.min(targetCount, availableSlots);

        if (count <= 0) return;

        if (count === 1) {
            const m = createMeteorInstance();
            if (m) meteors.push(m);
            return;
        }

        // count === 2: create two meteors with small offsets
        const offsetPosPx = 28; // horizontal offset between the two spawn points (px)
        const offsetYpx = 8;    // vertical offset
        const offsetSpreadDeg = 4; // +/- degrees difference

        const m1 = createMeteorInstance();
        if (m1) meteors.push(m1);

        const angleDegBase = meteorConfig.angleMinDeg + Math.random() * (meteorConfig.angleMaxDeg - meteorConfig.angleMinDeg);
        const angleDeg2 = angleDegBase + (Math.random() - 0.5) * offsetSpreadDeg;

        const m2Overrides = {
            startX: -offsetPosPx + (Math.random() - 0.5) * 12,
            startY: offsetYpx + (Math.random() - 0.5) * 8,
            angleDeg: angleDeg2
        };
        const m2 = createMeteorInstance(m2Overrides);
        if (m2) meteors.push(m2);
    }

    // schedule with jitter
    function scheduleNextSpawn() {
        const jitter = (Math.random() - 0.5) * meteorConfig.spawnJitter;
        const delay = Math.max(400, meteorConfig.spawnInterval + jitter);
        spawnTimer = setTimeout(() => {
            spawnBurst();
            scheduleNextSpawn();
        }, Math.round(delay));
    }

    function startMeteorSpawner() {
        if (spawnerStarted) return;
        spawnerStarted = true;
        scheduleNextSpawn();
    }

    // ---------- Drawing loop (stars + meteors) ----------
    function drawStarsAndMeteors(time) {
        if (!ctx) return;
        const w = canvas.width / (window.devicePixelRatio || 1);
        const h = canvas.height / (window.devicePixelRatio || 1);

        ctx.clearRect(0, 0, w, h);

        // subtle background (vignette-like)
        const gv = ctx.createLinearGradient(0, 0, 0, h);
        gv.addColorStop(0, 'rgba(255,255,255,0.02)');
        gv.addColorStop(1, 'rgba(0,0,0,0.02)');
        ctx.fillStyle = gv;
        ctx.fillRect(0, 0, w, h);

        const t = time * 0.001; // seconds

        // draw stars with enhanced twinkle + occasional spark
        for (let i = 0; i < stars.length; i++) {
            const s = stars[i];

            // periodic twinkle (sinus)
            let a = s.baseAlpha + Math.sin(t * s.twinkleSpeed + s.phase) * s.twinkleRange * 0.5;

            // handle occasional spark/pop: if we've reached next spark time, start it
            if (t >= s.sparkNext && (t - s.sparkNext) < 0.001) {
                s.sparkLast = t;
                // schedule next one: random delay 3..12s plus some size bias
                s.sparkNext = t + 3 + Math.random() * 9;
            }

            // compute spark contribution (decays linearly over sparkDuration)
            let sparkFactor = 0;
            if (s.sparkLast > -900) {
                const dt = t - s.sparkLast;
                if (dt >= 0 && dt <= s.sparkDuration) {
                    // ease-out curve for sparkle
                    const p = 1 - (dt / s.sparkDuration);
                    sparkFactor = Math.pow(p, 1.8) * s.sparkStrength; // stronger for large stars
                }
            }

            a += sparkFactor;
            a = Math.max(0, Math.min(1.8, a)); // cap to avoid extreme bright overflow

            // drawing glow: make glow radius increase during spark
            const gx = s.x;
            const gy = s.y;
            const baseGr = s.r * 6;
            const gr = baseGr * (1 + Math.min(0.9, sparkFactor * 0.9)); // increase glow up to ~1.9x

            const rad = ctx.createRadialGradient(gx, gy, 0, gx, gy, gr);
            const tint = s.hueShift;
            // tint and alpha mixture, slightly boosted by spark
            rad.addColorStop(0, `rgba(${255 + tint},${245 + tint * 0.6},${220 - tint * 0.6},${Math.min(1, a * 0.9)})`);
            rad.addColorStop(0.18, `rgba(255,255,255,${Math.min(1, a * 0.6)})`);
            rad.addColorStop(0.5, `rgba(200,200,220,${Math.min(1, a * 0.22)})`);
            rad.addColorStop(1, `rgba(120,140,160,${Math.min(0.2, a * 0.02)})`);

            ctx.globalCompositeOperation = 'screen';
            ctx.fillStyle = rad;
            ctx.beginPath();
            ctx.arc(gx, gy, gr, 0, Math.PI * 2);
            ctx.fill();

            // bright core, amplified during spark
            ctx.globalCompositeOperation = 'lighter';
            const coreAlpha = Math.min(1, a * (1 + sparkFactor * 0.6));
            ctx.fillStyle = `rgba(255,255,255,${coreAlpha})`;
            ctx.beginPath();
            ctx.arc(gx, gy, Math.max(0.6, s.r * (1 + sparkFactor * 0.6)), 0, Math.PI * 2);
            ctx.fill();
        }

        // draw meteors
        const now = t;
        for (let m = meteors.length - 1; m >= 0; m--) {
            const met = meteors[m];
            const age = now - met.created;
            const progress = age / met.life;

            if (progress > 1.05) {
                meteors.splice(m, 1);
                continue;
            }

            const cx = met.x + met.vx * age;
            const cy = met.y + met.vy * age;

            let alpha = meteorConfig.alpha * met.brightness;
            if (progress > met.fadeStart) {
                const fadeFactor = (1 - (progress - met.fadeStart) / (1 - met.fadeStart));
                alpha *= Math.max(0, fadeFactor);
            }

            const theta = Math.atan2(met.vy, met.vx);
            const tx = cx - Math.cos(theta) * met.length;
            const ty = cy - Math.sin(theta) * met.length;

            // trail gradient
            const grad = ctx.createLinearGradient(cx, cy, tx, ty);
            grad.addColorStop(0, `rgba(255,255,255,${alpha * 1.0})`);
            grad.addColorStop(0.25, `rgba(255,240,210,${alpha * 0.55})`);
            grad.addColorStop(0.6, `rgba(220,200,170,${alpha * 0.12})`);
            grad.addColorStop(1, `rgba(120,110,95,${alpha * 0.02})`);

            ctx.save();
            ctx.globalCompositeOperation = 'lighter';
            ctx.lineWidth = met.width * 1.6;
            ctx.strokeStyle = grad;
            ctx.beginPath();
            ctx.moveTo(cx, cy);
            ctx.lineTo(tx, ty);
            ctx.stroke();

            // bright core line
            ctx.lineWidth = Math.max(0.8, met.width);
            ctx.strokeStyle = `rgba(255,255,255,${alpha * 0.98})`;
            ctx.beginPath();
            ctx.moveTo(cx, cy);
            ctx.lineTo(tx, ty);
            ctx.stroke();

            // head glow
            const headR = Math.max(2.2, met.width * 2.8);
            const headGrad = ctx.createRadialGradient(cx, cy, 0, cx, cy, headR * 6);
            headGrad.addColorStop(0, `rgba(255,255,255,${alpha * 1.0})`);
            headGrad.addColorStop(0.2, `rgba(255,240,210,${alpha * 0.7})`);
            headGrad.addColorStop(0.5, `rgba(255,200,120,${alpha * 0.12})`);
            headGrad.addColorStop(1, 'rgba(0,0,0,0)');
            ctx.fillStyle = headGrad;
            ctx.beginPath();
            ctx.arc(cx, cy, headR * 5, 0, Math.PI * 2);
            ctx.fill();

            ctx.restore();

            if (cx < -80 || cy > h + 80 || alpha < 0.02) {
                meteors.splice(m, 1);
            }
        }
    }

    function animate(now) {
        if (!visible) {
            animId = requestAnimationFrame(animate);
            return;
        }
        drawStarsAndMeteors(now);
        animId = requestAnimationFrame(animate);
    }

    // Pause when tab hidden
    document.addEventListener('visibilitychange', function () {
        visible = document.visibilityState === 'visible';
    });

    // ---------- Initialization ----------
    if (canvas && ctx) {
        resizeCanvas();
        animId = requestAnimationFrame(animate);

        window.addEventListener('resize', function () {
            clearTimeout(canvas._resizeTimer);
            canvas._resizeTimer = setTimeout(resizeCanvas, 180);
        });
    }

    // ---------- Curtain & intro interaction (existing logic) ----------
    const curtains = document.getElementById('curtains');
    const left = curtains.querySelector('.curtain.left');
    const right = curtains.querySelector('.curtain.right');
    const content = document.querySelector('.intro-content');
    const continueBtn = document.getElementById('continueBtn');
    const mainTitleEl = document.querySelector('.main-title');
    const greetingEl = document.querySelector('.greeting');

    const computed = getComputedStyle(document.documentElement);
    const leftDuration = parseFloat(computed.getPropertyValue('--curtain-duration-left')) || 3.6;
    const rightDuration = parseFloat(computed.getPropertyValue('--curtain-duration-right')) || 4.0;
    const maxDuration = Math.max(leftDuration, rightDuration);

    const startDelay = 900;
    const fallbackTimeout = Math.round(maxDuration * 1000) + 1200;

    // disable continue until reveal
    continueBtn.classList.add('disabled');
    continueBtn.setAttribute('aria-disabled', 'true');

    setTimeout(function () {
        curtains.classList.add('opened');
    }, startDelay);

    let completed = 0;
    function onCurtainTransition(e) {
        if (e.propertyName && !/transform|opacity/.test(e.propertyName)) return;
        completed++;
        if (completed >= 2) {
            content.classList.add('revealed');
            if (mainTitleEl) mainTitleEl.classList.add('animate');
            if (greetingEl) greetingEl.classList.add('animate');

            // enable continue
            continueBtn.classList.remove('disabled');
            continueBtn.removeAttribute('aria-disabled');
            try { continueBtn.focus({ preventScroll: true }); } catch (err) { continueBtn.focus(); }

            // start meteors when intro revealed
            startMeteorSpawner();

            const approxTitleDuration = 900;
            setTimeout(function () {
                if (mainTitleEl) mainTitleEl.classList.add('floating');
            }, approxTitleDuration + 200);

            left.removeEventListener('transitionend', onCurtainTransition);
            right.removeEventListener('transitionend', onCurtainTransition);
        }
    }
    left.addEventListener('transitionend', onCurtainTransition);
    right.addEventListener('transitionend', onCurtainTransition);

    // ESC skip
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' || e.key === 'Esc') {
            if (!content.classList.contains('revealed')) {
                curtains.classList.add('opened');
                setTimeout(function () {
                    content.classList.add('revealed');
                    if (mainTitleEl) mainTitleEl.classList.add('animate');
                    if (greetingEl) greetingEl.classList.add('animate');
                    continueBtn.classList.remove('disabled');
                    continueBtn.removeAttribute('aria-disabled');
                    try { continueBtn.focus({ preventScroll: true }); } catch (err) { continueBtn.focus(); }

                    // also start meteors on skip
                    startMeteorSpawner();
                }, 700);
            }
        }
    });

    // Continue button: fade content, close curtains, then navigate
    continueBtn.addEventListener('click', function (ev) {
        if (continueBtn.classList.contains('disabled') || continueBtn.getAttribute('aria-disabled') === 'true') {
            ev.preventDefault(); return;
        }
        ev.preventDefault();
        const href = continueBtn.getAttribute('href');

        content.style.transition = 'opacity 0.45s ease, transform 0.45s cubic-bezier(.2,.9,.2,1)';
        content.style.opacity = '0';
        content.style.transform = 'translateY(-10px) scale(0.996)';

        setTimeout(function () {
            curtains.classList.remove('opened');
        }, 80);

        const closeWait = maxDuration;
        setTimeout(function () {
            window.location.href = href;
        }, Math.round(closeWait * 1000) + 260);
    });

    // Safety fallback: reveal content after timeout if transition events didn't fire
    setTimeout(function () {
        if (!content.classList.contains('revealed')) {
            content.classList.add('revealed');
            if (mainTitleEl) mainTitleEl.classList.add('animate');
            if (greetingEl) greetingEl.classList.add('animate');
            continueBtn.classList.remove('disabled');
            continueBtn.removeAttribute('aria-disabled');

            // start meteors as fallback
            startMeteorSpawner();
        }
    }, fallbackTimeout);

    // Cleanup on page unload
    window.addEventListener('beforeunload', function () {
        if (animId) cancelAnimationFrame(animId);
        if (spawnTimer) clearTimeout(spawnTimer);
    });
});