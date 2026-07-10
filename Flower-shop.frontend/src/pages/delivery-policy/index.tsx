import SEO from '../../components/SEO';
import React, { useEffect } from 'react';

const DeliveryPolicy: React.FC = () => {
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Chính sách giao hàng" description="Chính sách giao hàng của PDA Flower" />
      <main className="w-full max-w-[1440px] mx-auto px-margin-mobile md:px-margin-desktop py-16 md:py-20">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-16">
            <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary block mb-4">Chính sách</span>
            <h1 className="font-display-xl-mobile md:font-display-xl text-display-xl-mobile md:text-display-xl text-primary uppercase tracking-tight leading-none mb-6">
              GIAO HÀNG
            </h1>
            <div className="w-8 h-0.5 bg-primary mx-auto mb-6"></div>
            <p className="font-body-lg text-body-lg text-secondary">
              Cập nhật lần cuối: 10/07/2026
            </p>
          </div>

          <div className="space-y-12">
            <Section number="01" title="Mục đích">
              <p>
                Chính sách giao hàng được xây dựng nhằm quy định rõ quy trình giao nhận hoa tươi, thời gian giao hàng, trách nhiệm của các bên và các trường hợp phát sinh trong quá trình vận chuyển, đảm bảo quyền lợi của khách hàng và cửa hàng.
              </p>
            </Section>

            <Section number="02" title="Phạm vi giao hàng">
              <p>Website nhận giao hoa trong phạm vi các khu vực được hỗ trợ.</p>
              <p className="mt-4">
                Khi đặt hàng, khách hàng vui lòng nhập chính xác địa chỉ nhận hàng để hệ thống kiểm tra khả năng giao hàng.
              </p>
              <p className="mt-4">
                Đối với các khu vực ngoài phạm vi phục vụ, nhân viên sẽ chủ động liên hệ để tư vấn hoặc thông báo nếu không thể giao hàng.
              </p>
            </Section>

            <Section number="03" title="Thời gian giao hàng">
              <p>Thời gian giao hàng phụ thuộc vào địa chỉ nhận hàng, khung giờ khách hàng lựa chọn và tình trạng đơn hàng.</p>
              <p className="mt-4">Thông thường:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Giao trong ngày đối với các đơn hàng đặt trước thời gian quy định.</li>
                <li>Giao theo ngày và khung giờ khách hàng đã lựa chọn.</li>
                <li>Đơn hàng vào các dịp lễ, Tết hoặc ngày cao điểm (08/03, 20/10, Valentine, 20/11...) có thể kéo dài hơn do số lượng đơn tăng cao.</li>
              </ul>
              <p className="mt-4">Khách hàng sẽ được thông báo nếu có thay đổi về thời gian giao hàng.</p>
            </Section>

            <Section number="04" title="Khung giờ giao hàng">
              <p>Khách hàng có thể lựa chọn khung giờ giao hàng phù hợp khi đặt hàng.</p>
              <p className="mt-4">Ví dụ:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>08:00 - 10:00</li>
                <li>10:00 - 12:00</li>
                <li>13:00 - 15:00</li>
                <li>15:00 - 17:00</li>
                <li>17:00 - 19:00</li>
                <li>19:00 - 21:00</li>
              </ul>
              <p className="mt-4">
                Cửa hàng sẽ cố gắng giao đúng khung giờ đã chọn. Tuy nhiên, trong một số trường hợp khách quan như thời tiết xấu, ùn tắc giao thông hoặc lượng đơn hàng lớn, thời gian giao có thể thay đổi.
              </p>
            </Section>

            <Section number="05" title="Quy trình giao hàng">
              <p>Quy trình giao hàng được thực hiện theo các bước sau:</p>
              <ol className="list-decimal pl-5 space-y-2 text-secondary mt-4">
                <li>Khách hàng đặt đơn hàng trên website.</li>
                <li>Hệ thống xác nhận đơn hàng.</li>
                <li>Khách hàng thanh toán trực tuyến hoặc chọn thanh toán khi nhận hàng (nếu được hỗ trợ).</li>
                <li>Cửa hàng xác nhận thanh toán và bắt đầu chuẩn bị hoa.</li>
                <li>Nhân viên cắm hoa theo mẫu đã đặt.</li>
                <li>Kiểm tra chất lượng sản phẩm trước khi giao.</li>
                <li>Đóng gói cẩn thận nhằm đảm bảo hoa không bị hư hỏng trong quá trình vận chuyển.</li>
                <li>Bàn giao cho nhân viên giao hàng.</li>
                <li>Giao đến người nhận.</li>
                <li>Cập nhật trạng thái đơn hàng và gửi thông báo cho khách hàng.</li>
              </ol>
            </Section>

            <Section number="06" title="Phí giao hàng">
              <p>Phí giao hàng được tính dựa trên:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Khoảng cách giao hàng.</li>
                <li>Khu vực nhận hàng.</li>
                <li>Giá trị đơn hàng.</li>
                <li>Chương trình miễn phí vận chuyển (nếu có).</li>
              </ul>
              <p className="mt-4">Phí giao hàng sẽ được hiển thị trước khi khách hàng xác nhận thanh toán.</p>
            </Section>

            <Section number="07" title="Kiểm tra khi nhận hàng">
              <p>Khách hàng hoặc người nhận nên kiểm tra:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Đúng tên người nhận.</li>
                <li>Đúng mẫu hoa.</li>
                <li>Đúng số lượng sản phẩm.</li>
                <li>Hoa còn tươi và không bị dập nát.</li>
                <li>Phụ kiện đi kèm đầy đủ (thiệp, nơ, quà tặng... nếu có).</li>
              </ul>
              <p className="mt-4">Nếu phát hiện sai sót, vui lòng liên hệ ngay với cửa hàng để được hỗ trợ.</p>
            </Section>

            <Section number="08" title="Trường hợp giao hàng không thành công">
              <p>Đơn hàng có thể không giao thành công trong các trường hợp sau:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Không liên lạc được với người nhận.</li>
                <li>Người nhận từ chối nhận hoa.</li>
                <li>Địa chỉ giao hàng không chính xác hoặc không tồn tại.</li>
                <li>Khách hàng yêu cầu thay đổi địa chỉ khi đơn hàng đang được giao.</li>
                <li>Điều kiện thời tiết hoặc các sự cố bất khả kháng ảnh hưởng đến việc vận chuyển.</li>
              </ul>
              <p className="mt-4">Trong các trường hợp trên, nhân viên sẽ liên hệ với khách hàng để tìm phương án xử lý phù hợp.</p>
            </Section>

            <Section number="09" title="Thay đổi thông tin giao hàng">
              <p>Khách hàng có thể yêu cầu thay đổi:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Người nhận.</li>
                <li>Số điện thoại người nhận.</li>
                <li>Địa chỉ giao hàng.</li>
                <li>Ngày giao.</li>
                <li>Khung giờ giao.</li>
              </ul>
              <p className="mt-4">Việc thay đổi chỉ được hỗ trợ khi đơn hàng chưa chuyển sang trạng thái:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-2">
                <li>Đang chuẩn bị hoa.</li>
                <li>Đang cắm hoa.</li>
                <li>Đang giao hàng.</li>
              </ul>
              <p className="mt-4">Nếu đơn hàng đã bắt đầu thực hiện, việc thay đổi có thể không được chấp nhận hoặc phát sinh thêm chi phí.</p>
            </Section>

            <Section number="10" title="Trách nhiệm của khách hàng">
              <p>Khách hàng có trách nhiệm:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Cung cấp đầy đủ và chính xác thông tin người nhận.</li>
                <li>Đảm bảo người nhận có mặt trong thời gian giao hàng.</li>
                <li>Thanh toán đầy đủ theo phương thức đã lựa chọn.</li>
                <li>Thông báo ngay nếu phát hiện sai sót về đơn hàng.</li>
              </ul>
            </Section>

            <Section number="11" title="Trách nhiệm của cửa hàng">
              <p>Cửa hàng cam kết:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Chuẩn bị hoa đúng mẫu hoặc tương đương trong trường hợp bất khả kháng.</li>
                <li>Đảm bảo chất lượng hoa trước khi giao.</li>
                <li>Giao hàng đúng địa chỉ và trong thời gian đã cam kết (trừ các trường hợp bất khả kháng).</li>
                <li>Thông báo kịp thời nếu có sự thay đổi về sản phẩm hoặc thời gian giao.</li>
                <li>Hỗ trợ khách hàng nhanh chóng khi phát sinh sự cố trong quá trình giao hàng.</li>
              </ul>
            </Section>

            <Section number="12" title="Giao hàng trong các dịp lễ">
              <p>Trong các dịp lễ lớn như:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Lễ Tình nhân (Valentine)</li>
                <li>Quốc tế Phụ nữ (08/03)</li>
                <li>Ngày Phụ nữ Việt Nam (20/10)</li>
                <li>Ngày Nhà giáo Việt Nam (20/11)</li>
                <li>Ngày của Mẹ</li>
                <li>Ngày của Cha</li>
                <li>Tết Nguyên đán</li>
              </ul>
              <p className="mt-4">Số lượng đơn hàng tăng cao nên:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-2">
                <li>Khách hàng nên đặt trước để đảm bảo còn sản phẩm và khung giờ giao mong muốn.</li>
                <li>Cửa hàng có thể điều chỉnh thời gian giao trong phạm vi hợp lý.</li>
                <li>Một số loại hoa có thể được thay thế bằng loại tương đương nếu nguồn cung không đủ, nhưng vẫn đảm bảo giá trị và tính thẩm mỹ của sản phẩm.</li>
              </ul>
            </Section>

            <Section number="13" title="Các trường hợp bất khả kháng">
              <p>Cửa hàng không chịu trách nhiệm đối với việc giao hàng chậm hoặc không thể giao hàng do:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Thiên tai.</li>
                <li>Mưa bão, lũ lụt.</li>
                <li>Dịch bệnh.</li>
                <li>Tai nạn giao thông.</li>
                <li>Cấm đường.</li>
                <li>Sự cố hệ thống điện, Internet hoặc cổng thanh toán.</li>
                <li>Các sự kiện ngoài khả năng kiểm soát của cửa hàng.</li>
              </ul>
              <p className="mt-4">Trong các trường hợp này, cửa hàng sẽ chủ động liên hệ với khách hàng để thống nhất phương án xử lý phù hợp.</p>
            </Section>

            <Section number="14" title="Liên hệ hỗ trợ">
              <p>Nếu có bất kỳ thắc mắc hoặc cần hỗ trợ liên quan đến việc giao hàng, khách hàng vui lòng liên hệ:</p>
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
              Flower Shop luôn nỗ lực giao hoa đúng hẹn, đúng mẫu và đảm bảo chất lượng tốt nhất. Chúng tôi hiểu rằng mỗi bó hoa đều mang một thông điệp ý nghĩa, vì vậy việc giao hàng đúng thời gian và đến đúng người nhận luôn là ưu tiên hàng đầu của cửa hàng.
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

export default DeliveryPolicy;
