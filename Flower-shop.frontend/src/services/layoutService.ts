import axiosClient from '../api/axiosClient';

export interface TopBar {
  isActive: boolean;
  text?: string;
  url?: string | null;
}

export interface Zones {
  left: string[];
  center: string[];
  right: string[];
}

export interface CtaButton {
  isActive: boolean;
  text?: string | null;
  url?: string | null;
  variant?: string | null;
}

export interface HotlineConfig {
  useDefault: boolean;
  customText?: string | null;
}

export interface SearchConfig {
  mode: 'popup' | 'input';
}

export interface MenuItem {
  id: string;
  label: string;
  url: string;
  isExternal: boolean;
  children: MenuItem[];
}

export interface HeaderLayout {
  topBar: TopBar;
  zones: Zones;
  ctaButton: CtaButton;
  hotline: HotlineConfig;
  search: SearchConfig;
  menuItems: MenuItem[];
}

export interface FooterLink {
  id: string;
  label: string;
  type: 'page' | 'custom' | 'text_block';
  pageId?: number | null;
  url?: string | null;
}

export interface FooterColumn {
  title: string;
  align: 'left' | 'center' | 'right';
  sortOrder: number;
  type: 'links' | 'social_icons' | 'text_block';
  isActive: boolean;
  links: FooterLink[];
}

export interface LayoutResponse {
  header: HeaderLayout;
  footer: FooterColumn[];
  storeInfo: Record<string, unknown>;
}

const layoutService = {
  getLayout: async (): Promise<LayoutResponse> => {
    try {
      const response = await axiosClient.get<LayoutResponse>('/layout');
      return response as unknown as LayoutResponse;
    } catch (error) {
      console.error('Failed to fetch layout:', error);
      throw error;
    }
  },
};

export default layoutService;
