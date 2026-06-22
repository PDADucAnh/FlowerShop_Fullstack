import React from 'react';

const FloristNote: React.FC = () => {
  return (
    <section className="py-stack-lg md:py-[100px] bg-secondary-fixed/50 border-y border-outline-variant/30">
      <div className="px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto flex flex-col md:flex-row items-center gap-margin-desktop">
        <div className="w-full md:w-1/2 relative">
          <div className="aspect-[4/5] rounded-xl overflow-hidden petal-shadow">
            <img className="w-full h-full object-cover"
              alt="A portrait of a florist working in a bright, airy studio. The florist is focused on arranging a bouquet of pale pink roses and greenery. The studio has white walls, natural wood workbenches, and large windows letting in abundant natural light."
              src="https://lh3.googleusercontent.com/aida-public/AB6AXuABaPBnEieX3CA9FS_zJVFqxRI0-2OTB9GZ932ddmP6L7fRs_4-cJqj1vywEWvHFYv9GCzllqa0I1_2GxooJWM1HeFuPBAf9fONNuvArSj4Kd0_rLl1Fd-yiR9MYbI122CTq6xMC71gaz6AkJ6eQvsawzB3iYwRPtQid2L7SR4BuWxwxf6CCaZwXL6BfrPNz0qKwyXa2vvHg8MN-stYOueYbGpSs_THevk7wNkE7hRSjbnyqDbeoscaZhGba6NBWvMCFmfvlAkoBlo" />
          </div>
          <div className="absolute -bottom-6 -right-6 w-32 h-32 bg-primary-fixed rounded-full mix-blend-multiply opacity-50 blur-xl"></div>
        </div>
        <div className="w-full md:w-1/2 flex flex-col justify-center">
          <span className="material-symbols-outlined text-primary/40 text-[60px] mb-4">format_quote</span>
          <h2 className="font-headline-md text-headline-md text-on-surface mb-stack-md leading-snug">
            &ldquo;We believe that every stem has a story. Our arrangements are more than just flowers; they are ephemeral sculptures designed to capture fleeting moments of beauty and emotion.&rdquo;
          </h2>
          <div className="flex items-center gap-4 mt-stack-sm">
            <div className="w-12 h-12 rounded-full bg-surface-container-high overflow-hidden">
              <img className="w-full h-full object-cover"
                alt="A headshot of a female florist smiling warmly."
                src="https://lh3.googleusercontent.com/aida-public/AB6AXuD4spM0RADZ0ADpIwq-9yOOaHMaudmTlKvKVRAjIu_Nc7c9cmWQqiucxwSxWBobJN8Ad9zt-q1geNgiQp3fKxYUNHCm_0tmvjy2NIatYT-IuOhxpJcn6ca9D5DKEvME9eDFK6iu2EOsHtQi1e2XM0BRumdOQLAPxK-pPIvII_jhNrDZZPQrsnvoZS9Da_QOQjTlEADJuE75wMxUUthkLARrsQI-TxZvdRLMQqd4qRO5S-4vVFnlPEO_1cAQz6kN8s-9TWRNmlFxw1Y" />
            </div>
            <div>
              <p className="font-label-md text-label-md text-on-surface">Eleanor Vance</p>
              <p className="font-label-sm text-label-sm text-on-surface-variant">Lead Floral Designer</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default FloristNote;
