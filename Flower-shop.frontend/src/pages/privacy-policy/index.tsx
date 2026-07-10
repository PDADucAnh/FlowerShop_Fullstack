import SEO from '../../components/SEO';
import React, { useEffect } from 'react';

const PrivacyPolicy: React.FC = () => {
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Chính sách bảo mật" description="Chính sách bảo mật thông tin của PDA Flower" />
      <main className="w-full max-w-[1440px] mx-auto px-margin-mobile md:px-margin-desktop py-16 md:py-20">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-16">
            <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary block mb-4">Chính sách</span>
            <h1 className="font-display-xl-mobile md:font-display-xl text-display-xl-mobile md:text-display-xl text-primary uppercase tracking-tight leading-none mb-6">
              BẢO MẬT
            </h1>
            <div className="w-8 h-0.5 bg-primary mx-auto mb-6"></div>
            <p className="font-body-lg text-body-lg text-secondary">
              Cập nhật lần cuối: 10/07/2026
            </p>
          </div>

          <div className="space-y-12">
            <Section number="01" title="Mục đích">
              <p>
                Website bán hoa tươi cam kết bảo vệ quyền riêng tư và thông tin cá nhân của khách hàng. Chính sách bảo mật này giải thích cách chúng tôi thu thập, sử dụng, lưu trữ và bảo vệ thông tin của khách hàng khi sử dụng các dịch vụ trên website.
              </p>
              <p>
                Việc khách hàng truy cập và sử dụng website đồng nghĩa với việc đã đọc, hiểu và đồng ý với các nội dung trong Chính sách bảo mật này.
              </p>
            </Section>

            <Section number="02" title="Thông tin được thu thập">
              <SubSection title="2.1 Thông tin cá nhân">
                <ul className="list-disc pl-5 space-y-2 text-secondary">
                  <li>Họ và tên.</li>
                  <li>Địa chỉ email.</li>
                  <li>Số điện thoại.</li>
                  <li>Địa chỉ nhận hàng.</li>
                  <li>Thông tin tài khoản đăng nhập.</li>
                  <li>Lịch sử mua hàng.</li>
                  <li>Danh sách sản phẩm yêu thích.</li>
                </ul>
              </SubSection>

              <SubSection title="2.2 Thông tin đơn hàng">
                <ul className="list-disc pl-5 space-y-2 text-secondary">
                  <li>Mã đơn hàng.</li>
                  <li>Danh sách sản phẩm.</li>
                  <li>Thời gian đặt hàng.</li>
                  <li>Địa chỉ giao hàng.</li>
                  <li>Khung giờ giao hàng.</li>
                  <li>Ghi chú của khách hàng.</li>
                  <li>Trạng thái đơn hàng.</li>
                </ul>
              </SubSection>

              <SubSection title="2.3 Thông tin thanh toán">
                <p>Website chỉ lưu các thông tin cần thiết phục vụ quản lý giao dịch, bao gồm:</p>
                <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                  <li>Phương thức thanh toán.</li>
                  <li>Mã giao dịch.</li>
                  <li>Thời gian thanh toán.</li>
                  <li>Trạng thái thanh toán.</li>
                  <li>Ngân hàng thanh toán (nếu có).</li>
                </ul>
                <p className="mt-4">Website <strong>không lưu</strong>:</p>
                <ul className="list-disc pl-5 space-y-2 text-secondary mt-2">
                  <li>Số thẻ ngân hàng.</li>
                  <li>Mã CVV/CVC.</li>
                  <li>Mật khẩu Internet Banking.</li>
                  <li>Mã OTP.</li>
                  <li>Thông tin xác thực của thẻ.</li>
                </ul>
                <p className="mt-4">
                  Mọi thông tin thanh toán được xử lý trực tiếp thông qua cổng thanh toán VNPAY hoặc ngân hàng của khách hàng.
                </p>
              </SubSection>
            </Section>

            <Section number="03" title="Mục đích sử dụng thông tin">
              <p>Thông tin khách hàng được sử dụng nhằm:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Xác nhận đơn hàng.</li>
                <li>Giao hoa đúng người nhận.</li>
                <li>Liên hệ khi có vấn đề phát sinh.</li>
                <li>Cập nhật trạng thái đơn hàng.</li>
                <li>Xử lý thanh toán.</li>
                <li>Hỗ trợ đổi trả và hoàn tiền.</li>
                <li>Gửi hóa đơn điện tử.</li>
                <li>Gửi email xác nhận đơn hàng.</li>
                <li>Cải thiện chất lượng dịch vụ.</li>
                <li>Thống kê và phân tích hoạt động kinh doanh.</li>
              </ul>
              <p className="mt-4">Website không sử dụng thông tin khách hàng cho các mục đích trái pháp luật.</p>
            </Section>

            <Section number="04" title="Chia sẻ thông tin">
              <p>Website cam kết không bán, trao đổi hoặc cho thuê thông tin cá nhân của khách hàng.</p>
              <p className="mt-4">Thông tin chỉ được chia sẻ trong các trường hợp sau:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Theo yêu cầu của cơ quan nhà nước có thẩm quyền.</li>
                <li>Cho đơn vị vận chuyển để thực hiện giao hàng.</li>
                <li>Cho cổng thanh toán VNPAY nhằm xử lý giao dịch.</li>
                <li>Khi có sự đồng ý của khách hàng.</li>
              </ul>
            </Section>

            <Section number="05" title="Bảo mật thông tin">
              <p>Website áp dụng nhiều biện pháp nhằm đảm bảo an toàn dữ liệu:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Mật khẩu được mã hóa bằng BCrypt trước khi lưu vào cơ sở dữ liệu.</li>
                <li>Xác thực người dùng bằng JWT Authentication.</li>
                <li>Giao tiếp giữa trình duyệt và máy chủ sử dụng giao thức HTTPS.</li>
                <li>Phân quyền giữa khách hàng và quản trị viên.</li>
                <li>Kiểm tra quyền truy cập đối với các API.</li>
                <li>Sao lưu dữ liệu định kỳ.</li>
                <li>Ghi nhận nhật ký hệ thống (Audit Log) để theo dõi các thao tác quan trọng.</li>
              </ul>
            </Section>

            <Section number="06" title="Thanh toán trực tuyến">
              <p>
                Website hỗ trợ thanh toán trực tuyến thông qua cổng thanh toán VNPAY.
              </p>
              <p className="mt-4">
                Quá trình thanh toán được thực hiện trên hệ thống của VNPAY, do đó website không lưu thông tin thẻ ngân hàng hoặc thông tin đăng nhập ngân hàng của khách hàng.
              </p>
              <p className="mt-4">
                Các giao dịch đều được xác thực và mã hóa theo tiêu chuẩn bảo mật của VNPAY.
              </p>
            </Section>

            <Section number="07" title="Lưu thông tin thanh toán">
              <p>Để nâng cao trải nghiệm người dùng, website có thể lưu các thông tin sau:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Phương thức thanh toán đã sử dụng gần nhất.</li>
                <li>Tên ngân hàng.</li>
                <li>Loại thanh toán.</li>
                <li>Thông tin giao dịch trước đây.</li>
              </ul>
              <p className="mt-4">
                Website <strong>không lưu</strong> số thẻ ngân hàng, mã CVV hoặc OTP.
              </p>
              <p className="mt-4">
                Khách hàng có thể xóa thông tin thanh toán đã lưu bất kỳ lúc nào trong phần quản lý tài khoản (nếu chức năng được hỗ trợ).
              </p>
            </Section>

            <Section number="08" title="Cookie">
              <p>Website sử dụng Cookie nhằm:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Ghi nhớ trạng thái đăng nhập.</li>
                <li>Lưu giỏ hàng.</li>
                <li>Ghi nhớ ngôn ngữ và giao diện.</li>
                <li>Cải thiện tốc độ truy cập.</li>
                <li>Phân tích hành vi sử dụng website.</li>
              </ul>
              <p className="mt-4">
                Khách hàng có thể chủ động tắt Cookie trong trình duyệt, tuy nhiên một số chức năng của website có thể hoạt động không đầy đủ.
              </p>
            </Section>

            <Section number="09" title="Email và thông báo">
              <p>Website có thể gửi email cho khách hàng trong các trường hợp:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Đăng ký tài khoản.</li>
                <li>Xác thực email.</li>
                <li>Quên mật khẩu.</li>
                <li>Đặt hàng thành công.</li>
                <li>Thanh toán thành công.</li>
                <li>Thanh toán thất bại.</li>
                <li>Hủy đơn hàng.</li>
                <li>Hoàn tiền.</li>
                <li>Đơn hàng đã giao thành công.</li>
                <li>Thông báo khuyến mãi (nếu khách hàng đồng ý nhận).</li>
              </ul>
              <p className="mt-4">Khách hàng có thể từ chối nhận email quảng cáo bất cứ lúc nào.</p>
            </Section>

            <Section number="10" title="Quyền của khách hàng">
              <p>Khách hàng có quyền:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Xem thông tin cá nhân.</li>
                <li>Chỉnh sửa thông tin cá nhân.</li>
                <li>Đổi mật khẩu.</li>
                <li>Yêu cầu cập nhật thông tin.</li>
                <li>Yêu cầu khóa tài khoản.</li>
                <li>Yêu cầu xóa tài khoản theo quy định của website.</li>
                <li>Khiếu nại nếu thông tin cá nhân bị sử dụng sai mục đích.</li>
              </ul>
            </Section>

            <Section number="11" title="Thời gian lưu trữ dữ liệu">
              <p>
                Thông tin khách hàng được lưu trong thời gian tài khoản còn hoạt động hoặc theo quy định của pháp luật.
              </p>
              <p className="mt-4">
                Khi khách hàng yêu cầu xóa tài khoản, hệ thống sẽ xử lý theo chính sách quản lý dữ liệu của website và các quy định pháp luật hiện hành.
              </p>
            </Section>

            <Section number="12" title="Quyền và trách nhiệm của khách hàng">
              <p>Khách hàng có trách nhiệm:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Cung cấp thông tin chính xác khi đăng ký tài khoản.</li>
                <li>Bảo mật tài khoản và mật khẩu.</li>
                <li>Không chia sẻ thông tin đăng nhập cho người khác.</li>
                <li>Thông báo ngay cho website nếu phát hiện tài khoản bị truy cập trái phép.</li>
              </ul>
            </Section>

            <Section number="13" title="Thay đổi chính sách bảo mật">
              <p>
                Website có quyền cập nhật hoặc thay đổi Chính sách bảo mật để phù hợp với quy định của pháp luật và nhu cầu hoạt động.
              </p>
              <p className="mt-4">
                Mọi thay đổi sẽ được công bố trên website và có hiệu lực kể từ thời điểm được đăng tải.
              </p>
            </Section>

            <Section number="14" title="Thông tin liên hệ">
              <p>Nếu có bất kỳ thắc mắc nào liên quan đến Chính sách bảo mật hoặc việc xử lý dữ liệu cá nhân, khách hàng vui lòng liên hệ:</p>
              <ul className="space-y-2 text-secondary mt-4">
                <li>Website: PDA Flower</li>
                <li>Email: <a href="mailto:support@flowershop.vn" className="text-primary hover:underline">support@flowershop.vn</a></li>
                <li>Hotline: 1900 xxxx</li>
                <li>Thời gian làm việc: 08:00 – 21:00 tất cả các ngày trong tuần.</li>
              </ul>
            </Section>
          </div>

          <div className="mt-16 p-6 border border-outline-variant bg-surface-dim text-sm text-secondary leading-relaxed">
            <p className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-3">Cam kết</p>
            <p>
              Flower Shop luôn tôn trọng quyền riêng tư của khách hàng và áp dụng các biện pháp kỹ thuật, quản lý phù hợp nhằm bảo vệ an toàn thông tin cá nhân trong suốt quá trình khách hàng sử dụng dịch vụ trên website.
            </p>
          </div>
        </div>
      </main>
    </div>
  );
};

const Section: React.FC<{ number: string; title: string; children: React.ReactNode }> = ({ number, title, children }) => (
  <section>
    <div className="flex items-start gap-4 mb-6">
      <span className="text-[10px] font-bold uppercase tracking-[0.3em] text-outline shrink-0 mt-1">{number}</span>
      <h2 className="font-headline-sm text-headline-sm uppercase tracking-widest text-primary">{title}</h2>
    </div>
    <div className="ml-0 md:ml-12 space-y-4 text-secondary leading-relaxed">
      {children}
    </div>
  </section>
);

const SubSection: React.FC<{ title: string; children: React.ReactNode }> = ({ title, children }) => (
  <div className="mt-8">
    <h3 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">{title}</h3>
    <div className="space-y-3 text-secondary leading-relaxed">
      {children}
    </div>
  </div>
);

export default PrivacyPolicy;
