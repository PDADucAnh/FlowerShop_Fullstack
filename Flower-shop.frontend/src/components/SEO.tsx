import { Helmet, HelmetProvider } from 'react-helmet-async';

interface SEOProps {
  title: string;
  description?: string;
}

const SEO: React.FC<SEOProps> = ({ title, description }) => (
  <Helmet>
    <title>{title} | PDA Flower</title>
    {description && <meta name="description" content={description} />}
  </Helmet>
);

export { HelmetProvider, SEO };
export default SEO;
