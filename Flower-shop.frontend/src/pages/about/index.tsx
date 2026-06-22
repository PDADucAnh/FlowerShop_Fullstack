import React, { useEffect } from 'react';

const About: React.FC = () => {
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  return (
    <div className="bg-surface text-on-surface font-body-md antialiased min-h-screen">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        <div className="max-w-3xl mx-auto text-center mb-20 md:mb-28">
          <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary block mb-6">Contemporary Romance</span>
          <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface uppercase tracking-tight leading-none mb-8">
            FlowerShop
          </h1>
          <div className="w-8 h-0.5 bg-primary mx-auto mb-8"></div>
          <p className="font-body-lg text-body-lg text-secondary leading-relaxed max-w-2xl mx-auto">
            Crafting a new standard in contemporary floral design through meticulous attention to detail, 
            uncompromising quality, and a deep reverence for the art of botanical arrangement.
          </p>
        </div>

        <div className="max-w-4xl mx-auto space-y-20 md:space-y-28">
          <section className="grid grid-cols-1 md:grid-cols-5 gap-8 md:gap-12">
            <div className="md:col-span-2">
              <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-outline">Our Story</span>
            </div>
            <div className="md:col-span-3 space-y-6">
              <p className="font-headline-md text-headline-md italic leading-relaxed text-on-surface">
                Every stem has a story. Our arrangements are more than just flowers; they are ephemeral sculptures designed to capture fleeting moments of beauty and emotion.
              </p>
              <div className="space-y-4 text-secondary leading-relaxed">
                <p>
                  Founded with a singular vision to create arrangements that transcend seasons and trends, 
                  FlowerShop represents a return to the essential. Every collection is born from 
                  a dialogue between heritage craftsmanship and contemporary sensibility.
                </p>
                <p>
                  We believe in the power of reduction: stripping away the superfluous until only 
                  the purest expression of botanical design remains. Our studio operates at the intersection 
                  of precision horticulture and artistic freedom, where every stem tells a story.
                </p>
              </div>
            </div>
          </section>

          <section className="grid grid-cols-1 md:grid-cols-5 gap-8 md:gap-12">
            <div className="md:col-span-2">
              <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-outline">Our Values</span>
            </div>
            <div className="md:col-span-3 grid grid-cols-1 md:grid-cols-3 gap-8 md:gap-6">
              {[
                { title: 'Craftsmanship', desc: 'Each arrangement is a testament to the hands that made it. We honor the art of slow floristry through meticulous selection and premium botanical curation.' },
                { title: 'Curation', desc: 'Every collection is thoughtfully edited. We believe in fewer, better things — pieces that earn their place in your space through enduring relevance.' },
                { title: 'Authenticity', desc: 'We remain true to our vision. No fleeting trends, no compromise on quality. Our identity is defined by what we choose not to do as much as what we create.' },
              ].map((v) => (
                <div key={v.title} className="space-y-4">
                  <div className="w-8 h-px bg-primary"></div>
                  <h3 className="font-headline-sm text-headline-sm uppercase tracking-widest text-on-surface">{v.title}</h3>
                  <p className="text-secondary text-sm leading-relaxed">{v.desc}</p>
                </div>
              ))}
            </div>
          </section>

          <section className="grid grid-cols-1 md:grid-cols-5 gap-8 md:gap-12">
            <div className="md:col-span-2">
              <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-outline">The Studio</span>
            </div>
            <div className="md:col-span-3 border-t border-primary pt-8">
              <p className="text-secondary leading-relaxed">
                Based in the creative heart of the city, our studio is both workshop and sanctuary — 
                a space where ideas are sketched, blooms are studied, and arrangements take form through 
                patient collaboration between designer and artisan. Every piece that bears the FlowerShop 
                label passes through our studio, ensuring that what reaches you carries the integrity 
                of its origin.
              </p>
            </div>
          </section>
        </div>
      </main>
    </div>
  );
};

export default About;
