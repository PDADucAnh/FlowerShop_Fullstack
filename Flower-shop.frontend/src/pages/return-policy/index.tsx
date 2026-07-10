import SEO from '../../components/SEO';
import React, { useEffect } from 'react';

const ReturnPolicy: React.FC = () => {
  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20 min-h-screen">
      <SEO title="Chính sách đổi trả và hoàn tiền" description="Chính sách đổi trả và hoàn tiền của PDA Flower" />
      <main className="w-full max-w-[1440px] mx-auto px-margin-mobile md:px-margin-desktop py-16 md:py-20">
        <div className="max-w-4xl mx-auto">
          <div className="text-center mb-16">
            <span className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary block mb-4">Chính sách</span>
            <h1 className="font-display-xl-mobile md:font-display-xl text-display-xl-mobile md:text-display-xl text-primary uppercase tracking-tight leading-none mb-6">
              ĐỔI TRẢ & HOÀN TIỀN
            </h1>
            <div className="w-8 h-0.5 bg-primary mx-auto mb-6"></div>
            <p className="font-body-lg text-body-lg text-secondary">
              Cập nhật lần cuối: 10/07/2026
            </p>
          </div>

          <div className="space-y-12">
            <Section number="01" title="Mục đích">
              <p>
                Chính sách đổi trả và hoàn tiền được xây dựng nhằm đảm bảo quyền lợi của khách hàng khi mua hoa tươi trên website, đồng thời giúp cửa hàng xử lý các trường hợp phát sinh một cách minh bạch, công bằng và nhanh chóng.
              </p>
            </Section>

            <Section number="02" title="Điều kiện được đổi hoặc hoàn tiền">
              <p>Khách hàng được hỗ trợ đổi sản phẩm hoặc hoàn tiền trong các trường hợp sau:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Hoa giao không đúng mẫu đã đặt.</li>
                <li>Hoa giao sai màu sắc hoặc chủng loại so với đơn hàng.</li>
                <li>Thiếu sản phẩm hoặc phụ kiện đi kèm.</li>
                <li>Hoa bị hư hỏng, dập nát nghiêm trọng trong quá trình vận chuyển.</li>
                <li>Giao sai người nhận.</li>
                <li>Shop không thể thực hiện đơn hàng do hết nguyên liệu hoặc các nguyên nhân khách quan khác.</li>
              </ul>
              <p className="mt-4">
                Khách hàng cần liên hệ với cửa hàng trong vòng <strong>02 giờ</strong> kể từ thời điểm nhận hoa để được hỗ trợ.
              </p>
            </Section>

            <Section number="03" title="Trường hợp không áp dụng đổi trả">
              <p>Website không hỗ trợ đổi trả trong các trường hợp sau:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Khách hàng thay đổi ý định sau khi đơn hàng đã hoàn thành.</li>
                <li>Người nhận từ chối nhận hoa vì lý do cá nhân.</li>
                <li>Khách hàng cung cấp sai địa chỉ hoặc số điện thoại người nhận.</li>
                <li>Không liên hệ được người nhận nhiều lần theo quy định.</li>
                <li>Hoa đã được sử dụng hoặc bảo quản không đúng cách sau khi giao thành công.</li>
                <li>Các trường hợp bất khả kháng như thiên tai, dịch bệnh, cấm đường hoặc sự cố ngoài khả năng kiểm soát.</li>
              </ul>
            </Section>

            <Section number="04" title="Chính sách hủy đơn hàng">
              <SubSection title="4.1 Khách hàng hủy trước khi shop thực hiện đơn">
                <p>
                  Nếu đơn hàng vẫn ở trạng thái: <em>Chờ thanh toán, Chờ xác nhận, Đã xác nhận nhưng chưa cắm hoa</em> — khách hàng được phép hủy đơn.
                </p>
                <p className="mt-4">
                  Nếu khách đã thanh toán trực tuyến bằng VNPAY hoặc chuyển khoản, cửa hàng sẽ hoàn lại <strong>100% giá trị thanh toán</strong>.
                </p>
                <p className="mt-4">Thời gian hoàn tiền:</p>
                <ul className="list-disc pl-5 space-y-2 text-secondary mt-2">
                  <li>Từ 01 đến 03 ngày làm việc đối với chuyển khoản ngân hàng.</li>
                  <li>Theo quy định của ngân hàng hoặc VNPAY đối với giao dịch thanh toán trực tuyến (thông thường không quá 24 giờ sau khi cửa hàng xử lý hoàn tiền).</li>
                </ul>
              </SubSection>

              <SubSection title="4.2 Khách hàng hủy khi shop đã bắt đầu cắm hoa">
                <p>
                  Nếu trạng thái đơn hàng là: <em>Đang chuẩn bị hoa, Đang cắm hoa</em> — đơn hàng đã phát sinh chi phí nguyên vật liệu và nhân công.
                </p>
                <p className="mt-4">Khách hàng sẽ chịu phí hủy: <strong>30% tổng giá trị đơn hàng</strong>.</p>
                <p>
                  Phần còn lại (<strong>70%</strong>) sẽ được hoàn về tài khoản đã thanh toán trong vòng <strong>24 giờ</strong> sau khi cửa hàng xác nhận hủy đơn.
                </p>
                <p className="mt-4">Email hoàn tiền sẽ ghi rõ:</p>
                <ul className="list-disc pl-5 space-y-2 text-secondary mt-2">
                  <li>Giá trị đơn hàng.</li>
                  <li>Phần phí bị khấu trừ.</li>
                  <li>Lý do khấu trừ.</li>
                  <li>Số tiền được hoàn.</li>
                  <li>Thời gian hoàn tiền.</li>
                </ul>
              </SubSection>

              <SubSection title="4.3 Khách hàng hủy khi đơn đang giao">
                <p>
                  Nếu trạng thái đơn hàng là: <em>Đang giao</em> — đơn hàng đã phát sinh chi phí cắm hoa, vận chuyển và nhân sự.
                </p>
                <p className="mt-4">Khách hàng sẽ chịu phí hủy: <strong>50% tổng giá trị đơn hàng</strong>.</p>
                <p><strong>50%</strong> còn lại sẽ được hoàn trong vòng <strong>24 giờ</strong> sau khi xác nhận hủy.</p>
              </SubSection>
            </Section>

            <Section number="05" title="Shop hủy đơn hàng">
              <p>Website có quyền hủy đơn hàng trong các trường hợp:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Không đủ nguyên liệu để thực hiện.</li>
                <li>Hoa hết hàng.</li>
                <li>Không thể giao đúng thời gian khách yêu cầu.</li>
                <li>Địa chỉ giao hàng nằm ngoài phạm vi phục vụ.</li>
                <li>Xảy ra thiên tai hoặc các sự cố bất khả kháng.</li>
              </ul>
              <div className="mt-6 p-6 border border-outline-variant bg-surface-dim">
                <ul className="space-y-2 text-secondary">
                  <li>✓ Khách hàng được hoàn <strong>100% số tiền đã thanh toán</strong>.</li>
                  <li>✓ Không phát sinh bất kỳ khoản phí nào.</li>
                  <li>✓ Email sẽ được gửi tự động để thông báo lý do hủy đơn.</li>
                  <li>✓ Tiền sẽ được hoàn trong vòng <strong>24 giờ</strong> kể từ khi cửa hàng xác nhận hủy đơn.</li>
                </ul>
              </div>
            </Section>

            <Section number="06" title="Chính sách hoàn tiền">
              <p>
                Tiền hoàn sẽ được chuyển về đúng phương thức khách hàng đã thanh toán.
              </p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Thanh toán VNPAY → hoàn về tài khoản ngân hàng liên kết với VNPAY.</li>
                <li>Thanh toán chuyển khoản → hoàn về tài khoản ngân hàng khách cung cấp.</li>
              </ul>
              <p className="mt-4">
                Website không hoàn tiền bằng tiền mặt đối với các giao dịch thanh toán trực tuyến.
              </p>
            </Section>

            <Section number="07" title="Email thông báo">
              <p>Hệ thống sẽ tự động gửi email trong các trường hợp sau:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-4">
                <li>Đặt hàng thành công.</li>
                <li>Thanh toán thành công.</li>
                <li>Thanh toán thất bại.</li>
                <li>Hủy đơn hàng.</li>
                <li>Hoàn tiền.</li>
                <li>Đơn hàng đã được giao thành công.</li>
              </ul>
              <p className="mt-4">Email hoàn tiền sẽ bao gồm:</p>
              <ul className="list-disc pl-5 space-y-2 text-secondary mt-2">
                <li>Mã đơn hàng.</li>
                <li>Ngày hủy đơn.</li>
                <li>Lý do hủy.</li>
                <li>Phương thức thanh toán.</li>
                <li>Tổng giá trị đơn hàng.</li>
                <li>Phần phí bị khấu trừ (nếu có).</li>
                <li>Số tiền được hoàn.</li>
                <li>Thời gian hoàn tiền dự kiến.</li>
              </ul>
            </Section>

            <Section number="08" title="Thời gian xử lý">
              <div className="overflow-x-auto">
                <table className="w-full border-collapse">
                  <thead>
                    <tr className="border-b border-primary">
                      <th className="text-left py-3 pr-4 font-label-sm text-label-sm uppercase tracking-widest text-primary">Trường hợp</th>
                      <th className="text-left py-3 pl-4 font-label-sm text-label-sm uppercase tracking-widest text-primary">Thời gian</th>
                    </tr>
                  </thead>
                  <tbody className="text-secondary">
                    <tr className="border-b border-outline-variant">
                      <td className="py-3 pr-4">Xác nhận yêu cầu hủy</td>
                      <td className="py-3 pl-4">Trong vòng 01 giờ làm việc</td>
                    </tr>
                    <tr className="border-b border-outline-variant">
                      <td className="py-3 pr-4">Shop hủy đơn</td>
                      <td className="py-3 pl-4">Ngay sau khi xác nhận không thể thực hiện</td>
                    </tr>
                    <tr className="border-b border-outline-variant">
                      <td className="py-3 pr-4">Hoàn tiền khi khách hủy trước khi cắm hoa</td>
                      <td className="py-3 pl-4">Trong vòng 24 giờ</td>
                    </tr>
                    <tr className="border-b border-outline-variant">
                      <td className="py-3 pr-4">Hoàn tiền khi khách hủy trong quá trình cắm hoa</td>
                      <td className="py-3 pl-4">Trong vòng 24 giờ</td>
                    </tr>
                    <tr className="border-b border-outline-variant">
                      <td className="py-3 pr-4">Hoàn tiền khi shop hủy đơn</td>
                      <td className="py-3 pl-4">Trong vòng 24 giờ</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </Section>

            <Section number="09" title="Liên hệ hỗ trợ">
              <p>Nếu có thắc mắc về việc đổi trả hoặc hoàn tiền, khách hàng vui lòng liên hệ:</p>
              <ul className="space-y-2 text-secondary mt-4">
                <li>Hotline: 1900 xxxx</li>
                <li>Email: <a href="mailto:support@flowershop.vn" className="text-primary hover:underline">support@flowershop.vn</a></li>
                <li>Thời gian làm việc: 08:00 – 21:00 tất cả các ngày trong tuần.</li>
              </ul>
            </Section>
          </div>

          <div className="mt-16 p-6 border border-outline-variant bg-surface-dim text-sm text-secondary leading-relaxed">
            <p className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-3">Lưu ý</p>
            <p>
              Chính sách này được xây dựng phù hợp với đặc thù của sản phẩm hoa tươi. Do hoa là mặt hàng dễ hư hỏng và có vòng đời ngắn, việc đổi trả và hoàn tiền sẽ được xem xét dựa trên tình trạng thực tế của đơn hàng nhằm đảm bảo quyền lợi hài hòa giữa khách hàng và cửa hàng.
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

export default ReturnPolicy;
