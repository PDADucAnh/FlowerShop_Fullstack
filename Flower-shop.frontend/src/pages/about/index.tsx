import SEO from '../../components/SEO';
import React, { useEffect } from 'react';

const About: React.FC = () => {
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Giới thiệu" description="Về PDA Flower" />
      <main className="w-full max-w-[1440px] mx-auto px-margin-mobile md:px-margin-desktop py-16 md:py-20">
        <div className="max-w-3xl mx-auto text-center mb-20 md:mb-28">
          <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary block mb-6">Thương hiệu hoa tươi</span>
          <h1 className="font-display-xl-mobile md:font-display-xl text-display-xl-mobile md:text-display-xl text-primary uppercase tracking-tight leading-none mb-8">
            PDA FLOWER<span className="text-outline font-body-md text-body-md tracking-[0.05em] font-normal lowercase">.Flower</span>
          </h1>
          <div className="w-8 h-0.5 bg-primary mx-auto mb-8"></div>
          <p className="font-body-lg text-body-lg text-secondary leading-relaxed max-w-2xl mx-auto">
            Nâng tầm nghệ thuật cắm hoa qua từng chi tiết tỉ mỉ, chất lượng không khoan nhượng 
            và tình yêu sâu sắc dành cho vẻ đẹp của thiên nhiên.
          </p>
        </div>

        <div className="max-w-4xl mx-auto space-y-20 md:space-y-28">
          <section className="grid grid-cols-1 md:grid-cols-5 gap-8 md:gap-12">
            <div className="md:col-span-2">
              <span className="text-[10px] font-bold uppercase tracking-[0.3em] text-outline">Câu chuyện</span>
            </div>
            <div className="md:col-span-3 space-y-6">
              <p className="serif text-2xl md:text-3xl italic leading-relaxed text-primary font-display-xl">
                Hoa không chỉ là món quà. Đó là ngôn ngữ của trái tim, một lời nhắn gửi yêu thương, 
                một niềm vui thầm lặng sưởi ấm tâm hồn người nhận.
              </p>
              <div className="space-y-4 text-secondary leading-relaxed">
                <p>
                  Được thành lập với một tầm nhìn duy nhất — tạo ra những tác phẩm hoa vượt thời gian — 
                  PDA FLOWER mang đến vẻ đẹp tinh túy nhất. Mỗi bó hoa là sự kết hợp giữa 
                  nghệ thuật cắm hoa truyền thống và cảm quan hiện đại.
                </p>
                <p>
                  Chúng tôi tin vào sức mạnh của sự giản đơn: loại bỏ những thứ thừa thãi để chỉ còn lại 
                  vẻ đẹp thuần khiết nhất. Mỗi tác phẩm hoa của chúng tôi đều là câu chuyện 
                  về sự tận tâm và tình yêu thiên nhiên.
                </p>
              </div>
            </div>
          </section>

          <section className="grid grid-cols-1 md:grid-cols-5 gap-8 md:gap-12">
            <div className="md:col-span-2">
              <span className="text-[10px] font-bold uppercase tracking-[0.3em] text-outline">Giá trị</span>
            </div>
            <div className="md:col-span-3 grid grid-cols-1 md:grid-cols-3 gap-8 md:gap-6">
              {[
                { title: 'Tinh xảo', desc: 'Mỗi sản phẩm là minh chứng cho bàn tay tạo ra nó. Chúng tôi tôn vinh nghệ thuật thời trang chậm thông qua chế tác tỉ mỉ và lựa chọn chất liệu cao cấp.' },
                { title: 'Tuyển chọn', desc: 'Mỗi bộ sưu tập đều được biên tập kỹ lưỡng. Chúng tôi tin vào ít hơn nhưng tốt hơn — những sản phẩm xứng đáng có vị trí trong tủ đồ của bạn.' },
                { title: 'Chân thực', desc: 'Chúng tôi luôn trung thành với tầm nhìn của mình. Không chạy theo xu hướng, không thỏa hiệp về chất lượng.' },
              ].map((v) => (
                <div key={v.title} className="space-y-4">
                  <div className="w-8 h-px bg-primary"></div>
                  <h3 className="font-headline-sm text-headline-sm uppercase tracking-widest text-primary">{v.title}</h3>
                  <p className="text-secondary text-sm leading-relaxed">{v.desc}</p>
                </div>
              ))}
            </div>
          </section>

          <section className="grid grid-cols-1 md:grid-cols-5 gap-8 md:gap-12">
            <div className="md:col-span-2">
              <span className="text-[10px] font-bold uppercase tracking-[0.3em] text-outline">Cửa hàng</span>
            </div>
            <div className="md:col-span-3 border-t border-primary pt-8">
              <p className="text-secondary leading-relaxed">
                Tọa lạc tại trung tâm thành phố, cửa hàng của chúng tôi là không gian nơi 
                những ý tưởng được ấp ủ, những loài hoa được tuyển chọn kỹ lưỡng và mỗi bó hoa 
                đều được tạo nên từ sự tỉ mỉ của người nghệ nhân. Mỗi sản phẩm mang thương hiệu 
                PDA FLOWER đều được chăm chút bởi bàn tay tài hoa, đảm bảo vẻ đẹp và chất lượng 
                tốt nhất đến tay bạn.
              </p>
            </div>
          </section>
        </div>
      </main>
    </div>
  );
};

export default About;
