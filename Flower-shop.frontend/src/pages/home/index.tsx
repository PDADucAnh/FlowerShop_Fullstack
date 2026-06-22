import React from 'react';
import HeroBanner from './HeroBanner';
import FeaturedCollections from './FeaturedCollections';
import FloristNote from './FloristNote';

const Home: React.FC = () => {
  return (
    <div className="flex flex-col">
      <HeroBanner />
      <FeaturedCollections />
      <FloristNote />
    </div>
  );
};

export default Home;
