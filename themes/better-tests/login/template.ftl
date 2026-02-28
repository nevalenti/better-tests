<#macro registrationLayout displayMessage=false displayRequiredFields=false>
  <!DOCTYPE html>
  <html lang="en" data-theme="emerald">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title><#if pageTitle??>${pageTitle}<#else>Keycloak</#if></title>
    <script src="https://cdn.jsdelivr.net/npm/@tailwindcss/browser@4"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/daisyui@5" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/daisyui@5/themes.css" />
    <style>
      @import url('https://rsms.me/inter/inter.css');
      @import url('https://fonts.googleapis.com/css2?family=Raleway:wght@600;700;800&display=swap');

      :root {
        --font-sans: 'Inter', ui-sans-serif, system-ui, sans-serif;
        --font-display: 'Raleway', ui-sans-serif, system-ui, sans-serif;
      }

      html {
        font-family: var(--font-sans);
      }

      h1, h2, h3, h4, h5, h6 {
        font-family: var(--font-display);
      }

      .swap span {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
      }

      .swap svg {
        width: 1.5rem;
        height: 1.5rem;
      }

      .floating-label {
        @apply relative block;
      }

      .floating-label input {
        @apply transition-all duration-200;
      }

      .floating-label input:focus {
        @apply ring-2 ring-primary/20;
      }

      .floating-label span {
        @apply absolute left-4 top-1/2 -translate-y-1/2 text-base-content/60 pointer-events-none transition-all duration-200 text-sm font-medium;
      }

      .floating-label input:focus ~ span,
      .floating-label input:not(:placeholder-shown) ~ span {
        @apply -top-3 left-4 text-xs text-primary bg-base-100 px-1;
      }

      .input:focus {
        @apply border-primary;
      }

      .btn:disabled {
        @apply opacity-50 cursor-not-allowed;
      }

      @keyframes slideUp {
        from {
          @apply opacity-0 translate-y-4;
        }
        to {
          @apply opacity-100 translate-y-0;
        }
      }

      .card {
        animation: slideUp 0.4s ease-out;
      }

      .form-group {
        @apply space-y-2;
      }

      .error-icon {
        @apply inline-block mr-2;
      }

      .card {
        @apply backdrop-blur-sm;
      }

      .input:focus {
        @apply ring-2 ring-primary/30 ring-offset-2 ring-offset-base-100;
      }

      .btn {
        @apply transition-all duration-200;
      }

      .btn:hover:not(:disabled) {
        @apply -translate-y-0.5;
      }

      .btn-link {
        @apply text-primary hover:text-primary-focus no-underline hover:underline;
      }

      .divider {
        @apply my-5;
      }

      @keyframes fadeInUp {
        from {
          @apply opacity-0 translate-y-6;
        }
        to {
          @apply opacity-100 translate-y-0;
        }
      }

      .form-group {
        animation: fadeInUp 0.3s ease-out;
      }

      .form-group:nth-child(1) { animation-delay: 0.05s; }
      .form-group:nth-child(2) { animation-delay: 0.1s; }
      .form-group:nth-child(3) { animation-delay: 0.15s; }
      .form-group:nth-child(4) { animation-delay: 0.2s; }
      .form-group:nth-child(5) { animation-delay: 0.25s; }

      .btn-primary {
        @apply shadow-md hover:shadow-lg;
      }
    </style>
    <script>
      function initTheme() {
        const savedTheme = localStorage.getItem('theme');
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        const theme = savedTheme || (prefersDark ? 'dim' : 'emerald');
        document.documentElement.setAttribute('data-theme', theme);
        updateThemeIcon(theme);
      }

      function updateThemeIcon(theme) {
        const sunIcon = document.getElementById('theme-sun');
        const moonIcon = document.getElementById('theme-moon');
        if (sunIcon && moonIcon) {
          if (theme === 'dim') {
            moonIcon.style.display = 'none';
            sunIcon.style.display = 'block';
          } else {
            moonIcon.style.display = 'block';
            sunIcon.style.display = 'none';
          }
        }
      }

      function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dim' ? 'emerald' : 'dim';
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon(newTheme);
      }

      document.addEventListener('DOMContentLoaded', initTheme);
    </script>
  </head>
  <body class="bg-base-100 text-base-content">
    <div class="mx-auto flex min-h-screen w-full max-w-360 flex-col">
      <nav class="navbar border-base-content/10 h-16 border-b px-4">
        <div class="navbar-start">
          <a
            href="http://localhost:4200"
            class="text-base-content text-xl font-extrabold whitespace-nowrap shrink-0 transition-opacity hover:opacity-60"
            style="font-family: var(--font-display)"
          >
            Better Tests
          </a>
        </div>
        <div class="navbar-end gap-2">
          <a href="${url.loginUrl}" class="btn btn-ghost btn-sm">Log in</a>
          <#if realm.registrationAllowed>
            <a href="${url.registrationUrl}" class="btn btn-primary btn-sm">Sign up</a>
          </#if>
        </div>
      </nav>

      <main class="flex flex-1 flex-col items-center justify-start p-4 lg:pt-[12vh]">
        <div class="w-full max-w-sm">
          <#nested "header">
        </div>
      </main>

      <footer class="footer footer-center text-base-content border-base-content/10 border-t p-4 sm:p-6 relative">
        <aside class="text-center text-sm w-full">
          <p class="text-sm font-extrabold" style="font-family: var(--font-display)">
            Better Tests
          </p>
          <p class="text-xs">Copyright © 2026 - All rights reserved</p>
        </aside>
        <div class="absolute right-4 sm:right-6 top-1/2 -translate-y-1/2">
          <button
            class="theme-toggle btn btn-ghost btn-circle inline-flex items-center justify-center"
            onclick="toggleTheme()"
            aria-label="Toggle theme"
          >
            <svg id="theme-sun" class="theme-icon size-6 fill-current" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="display: block;">
              <path d="M5.64,17l-.71.71a1,1,0,0,0,0,1.41,1,1,0,0,0,1.41,0l.71-.71A1,1,0,0,0,5.64,17ZM5,12a1,1,0,0,0-1-1H3a1,1,0,0,0,0,2H4A1,1,0,0,0,5,12Zm7-7a1,1,0,0,0,1-1V3a1,1,0,0,0-2,0V4A1,1,0,0,0,12,5ZM5.64,7.05a1,1,0,0,0,.7.29,1,1,0,0,0,0-1.41l-.71-.71A1,1,0,0,0,4.93,6.34Zm12,.29a1,1,0,0,0,.7-.29l.71-.71a1,1,0,1,0-1.41-1.41L17,5.64a1,1,0,0,0,0,1.41A1,1,0,0,0,17.66,7.34ZM21,11H20a1,1,0,0,0,0,2h1a1,1,0,0,0,0-2Zm-9,8a1,1,0,0,0-1,1v1a1,1,0,0,0,2,0V20A1,1,0,0,0,12,19ZM18.36,17A1,1,0,0,0,17,18.36l.71.71a1,1,0,0,0,1.41,0,1,1,0,0,0,0-1.41ZM12,6.5A5.5,5.5,0,1,0,17.5,12,5.51,5.51,0,0,0,12,6.5Zm0,9A3.5,3.5,0,1,1,15.5,12,3.5,3.5,0,0,1,12,15.5Z" />
            </svg>
            <svg id="theme-moon" class="theme-icon size-6 fill-current" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" style="display: none; position: absolute;">
              <path d="M21.64,13a1,1,0,0,0-1.05-.14,8.05,8.05,0,0,1-3.37.73A8.15,8.15,0,0,1,9.08,5.49a8.59,8.59,0,0,1,.25-2A1,1,0,0,0,8,2.36,10.14,10.14,0,1,0,22,14.05,1,1,0,0,0,21.64,13Zm-9.5,6.69A8.14,8.14,0,0,1,7.08,5.22v.27A10.15,10.15,0,0,0,17.22,15.63a9.79,9.79,0,0,0,2.1-.22A8.11,8.11,0,0,1,12.14,19.69Z" />
            </svg>
          </button>
        </div>
      </footer>
    </div>
  </body>
  </html>
</#macro>
