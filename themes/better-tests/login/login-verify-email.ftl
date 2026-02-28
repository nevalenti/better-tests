<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=displayMessage>
  <div class="card from-base-100 to-base-200/30 border-base-content/10 w-full max-w-sm rounded-lg border bg-linear-to-b shadow-lg">
    <div class="card-body px-8 py-10">
      <div class="mb-10 text-center">
        <div class="flex justify-center mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-16 h-16 text-info opacity-80" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>
        </div>
        <h1 class="text-5xl font-extrabold whitespace-nowrap" style="font-family: var(--font-display)">Better Tests</h1>
        <p class="mt-4 text-center text-sm opacity-70">Almost there! Let's confirm your email address</p>
      </div>

      <#if message??>
        <div class="alert alert-info alert-outline border-dashed mb-6 gap-3">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-5 w-5" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          <span class="text-sm">${message.summary}</span>
        </div>
      </#if>

      <div class="bg-base-200/50 rounded-lg p-5 mb-8">
        <p class="text-center text-sm opacity-80">
          We've sent a verification link to your email address. Please check your inbox and click the link to complete your registration.
        </p>
      </div>

      <form id="kc-verify-email-form" action="${url.loginAction}" method="post">
        <button
          type="submit"
          class="btn btn-primary w-full font-semibold text-sm"
        >
          Resend Email
        </button>
      </form>

      <div class="divider">OR</div>

      <a href="${url.loginUrl}" class="link link-primary font-semibold text-center block">
        Back to Login
      </a>
    </div>
  </div>
</@layout.registrationLayout>
