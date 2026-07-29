export interface TopBarDTO {
  isActive: boolean
  text?: string
  url?: string
}

export interface ZonesDTO {
  left: string[]
  center: string[]
  right: string[]
}

export interface CtaButtonDTO {
  isActive: boolean
  text?: string
  url?: string
  variant?: string
}

export interface HotlineConfigDTO {
  useDefault: boolean
  customText?: string
}

export interface SearchConfigDTO {
  mode: string
}

export interface MenuItemDTO {
  id: string
  label: string
  url: string
  isExternal?: boolean
  children?: MenuItemDTO[]
}

export interface FooterLinkDTO {
  id: string
  label: string
  type: string
  pageId?: number
  url?: string
}

export interface FooterColumnDTO {
  title: string
  align: string
  sortOrder: number
  type: string
  isActive: boolean
  links: FooterLinkDTO[]
}

export interface HeaderLayoutDTO {
  topBar: TopBarDTO
  zones: ZonesDTO
  ctaButton: CtaButtonDTO
  hotline: HotlineConfigDTO
  search: SearchConfigDTO
  menuItems: MenuItemDTO[]
}

export interface LayoutResponse {
  header: HeaderLayoutDTO
  footer: FooterColumnDTO[]
  storeInfo: StoreInfoSettings
}

import type { StoreInfoSettings } from './settings'
export type { StoreInfoSettings }
