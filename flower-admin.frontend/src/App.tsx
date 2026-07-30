import { BrowserRouter, Routes, Route } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Toaster } from 'sonner'
import { AuthProvider } from '@/context/AuthContext'
import { ProtectedRoute } from '@/components/ProtectedRoute'
import { AppShell } from '@/layouts/AppShell'
import { LoginPage } from '@/pages/LoginPage'
import { DashboardPage } from '@/pages/DashboardPage'
import { ProductsPage } from '@/pages/products/ProductsPage'
import { ProductFormPage } from '@/pages/products/ProductFormPage'
import { CategoriesPage } from '@/pages/categories/CategoriesPage'
import { ImportPage } from '@/pages/imports/ImportPage'
import { OrdersPage } from '@/pages/orders/OrdersPage'
import { OrderDetailPage } from '@/pages/orders/OrderDetailPage'
import { OrderCreatePage } from '@/pages/orders/OrderCreatePage'
import { CustomersPage } from '@/pages/customers/CustomersPage'
import { CustomerDetailPage } from '@/pages/customers/CustomerDetailPage'
import { ContactsPage } from '@/pages/contacts/ContactsPage'
import { ContactDetailPage } from '@/pages/contacts/ContactDetailPage'
import { ContentPage } from '@/pages/ContentPage'
import { PostFormPage } from '@/pages/content/PostFormPage'
import { PageFormPage } from '@/pages/content/PageFormPage'
import { MarketingPage } from '@/pages/MarketingPage'
import { SystemSettingsPage } from '@/pages/SystemSettingsPage'
import { UsersPage } from '@/pages/users/UsersPage'
import { NotificationsPage } from '@/pages/notifications/NotificationsPage'

const queryClient = new QueryClient()

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route element={<ProtectedRoute />}>
              <Route element={<AppShell />}>
                <Route index element={<DashboardPage />} />
                <Route path="orders" element={<OrdersPage />} />
                <Route path="orders/new" element={<OrderCreatePage />} />
                <Route path="orders/:id" element={<OrderDetailPage />} />
                <Route path="customers" element={<CustomersPage />} />
                <Route path="customers/:id" element={<CustomerDetailPage />} />
                <Route path="contacts" element={<ContactsPage />} />
                <Route path="contacts/:id" element={<ContactDetailPage />} />
                <Route path="products" element={<ProductsPage />} />
                <Route path="products/new" element={<ProductFormPage />} />
                <Route path="products/:id/edit" element={<ProductFormPage />} />
                <Route path="products/categories" element={<CategoriesPage />} />
                <Route path="products/import" element={<ImportPage />} />
                <Route path="content" element={<ContentPage />}>
                  <Route path="posts/new" element={<PostFormPage />} />
                  <Route path="posts/:id/edit" element={<PostFormPage />} />
                  <Route path="pages/new" element={<PageFormPage />} />
                  <Route path="pages/:id/edit" element={<PageFormPage />} />
                </Route>
                <Route path="marketing" element={<MarketingPage />} />
                <Route path="users" element={<UsersPage />} />
                <Route path="notifications" element={<NotificationsPage />} />
                <Route path="system" element={<SystemSettingsPage />} />
              </Route>
            </Route>
          </Routes>
        </AuthProvider>
      </BrowserRouter>
      <Toaster position="top-right" richColors />
    </QueryClientProvider>
  )
}

export default App
