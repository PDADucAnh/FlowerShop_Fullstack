import { useState, useEffect } from 'react';
import { Helmet } from 'react-helmet-async';
import settingsService, { StoreInfo } from '../../services/settingsService';
import contactService from '../../services/contactService';

const ContactPage = () => {
  const [storeInfo, setStoreInfo] = useState<StoreInfo | null>(null);
  const [form, setForm] = useState({ name: '', email: '', phone: '', subject: '', message: '' });
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    settingsService.getStoreInfo().then(res => setStoreInfo(res)).catch(() => {});
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setSuccess('');
    setError('');
    try {
      await contactService.submit(form);
      setSuccess('Cảm ơn bạn đã gửi liên hệ. Chúng tôi sẽ phản hồi sớm nhất.');
      setForm({ name: '', email: '', phone: '', subject: '', message: '' });
    } catch {
      setError('Gửi liên hệ thất bại. Vui lòng thử lại.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <>
      <Helmet><title>Liên hệ - FlowerShop</title></Helmet>
      <section className="py-16 px-4 max-w-6xl mx-auto">
        <h1 className="text-4xl font-bold text-center mb-4">Liên hệ</h1>
        <p className="text-gray-500 text-center mb-12 max-w-xl mx-auto">
          Hãy gửi cho chúng tôi bất kỳ câu hỏi hoặc thắc mắc nào. Chúng tôi luôn sẵn lòng hỗ trợ bạn.
        </p>

        <div className="grid md:grid-cols-2 gap-12">
          <div className="space-y-6">
            {storeInfo && (
              <>
                <div className="flex gap-4 items-start">
                  <span className="material-symbols-outlined text-primary">location_on</span>
                  <div><p className="font-semibold">Địa chỉ</p><p className="text-gray-500">{storeInfo.address}</p></div>
                </div>
                <div className="flex gap-4 items-start">
                  <span className="material-symbols-outlined text-primary">phone</span>
                  <div><p className="font-semibold">Hotline</p><p className="text-gray-500">{storeInfo.hotline}</p></div>
                </div>
                <div className="flex gap-4 items-start">
                  <span className="material-symbols-outlined text-primary">mail</span>
                  <div><p className="font-semibold">Email</p><p className="text-gray-500">{storeInfo.email}</p></div>
                </div>
                <div className="flex gap-4 items-start">
                  <span className="material-symbols-outlined text-primary">schedule</span>
                  <div><p className="font-semibold">Giờ làm việc</p><p className="text-gray-500">{storeInfo.openHours}</p></div>
                </div>
              </>
            )}
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            {success && <div className="bg-green-100 text-green-700 px-4 py-3 rounded-lg">{success}</div>}
            {error && <div className="bg-red-100 text-red-700 px-4 py-3 rounded-lg">{error}</div>}
            <div className="grid grid-cols-2 gap-4">
              <input placeholder="Họ tên *" value={form.name} onChange={e => setForm({...form, name: e.target.value})} required className="w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent outline-none" />
              <input placeholder="Email *" type="email" value={form.email} onChange={e => setForm({...form, email: e.target.value})} required className="w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent outline-none" />
            </div>
            <input placeholder="Số điện thoại" value={form.phone} onChange={e => setForm({...form, phone: e.target.value})} className="w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent outline-none" />
            <input placeholder="Chủ đề *" value={form.subject} onChange={e => setForm({...form, subject: e.target.value})} required className="w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent outline-none" />
            <textarea placeholder="Nội dung *" rows={5} value={form.message} onChange={e => setForm({...form, message: e.target.value})} required className="w-full px-4 py-3 border rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent outline-none"></textarea>
            <button type="submit" disabled={submitting} className="w-full bg-primary text-white py-3 rounded-lg font-bold hover:opacity-90 disabled:opacity-50">
              {submitting ? 'Đang gửi...' : 'Gửi liên hệ'}
            </button>
          </form>
        </div>
      </section>
    </>
  );
};

export default ContactPage;
