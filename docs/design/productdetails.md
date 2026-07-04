<!DOCTYPE html>

<html class="light" lang="en"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Pink Rose Harmony - FlowerShop</title>
<!-- Google Fonts -->
<link href="https://fonts.googleapis.com" rel="preconnect"/>
<link crossorigin="" href="https://fonts.gstatic.com" rel="preconnect"/>
<link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@600;700&amp;family=Plus+Jakarta+Sans:wght@400;500;600&amp;display=swap" rel="stylesheet"/>
<!-- Material Symbols -->
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:wght,FILL@100..700,0..1&amp;display=swap" rel="stylesheet"/>
<!-- Tailwind CSS -->
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<!-- Theme Configuration -->
<script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    "colors": {
                        "primary": "#ab2c5d",
                        "on-secondary-container": "#716066",
                        "surface-container-highest": "#e4e2e1",
                        "on-surface-variant": "#574146",
                        "surface-container-lowest": "#ffffff",
                        "error-container": "#ffdad6",
                        "on-error": "#ffffff",
                        "surface-tint": "#ab2c5d",
                        "surface": "#fbf9f8",
                        "on-tertiary": "#ffffff",
                        "secondary-fixed": "#f4dce4",
                        "background": "#fbf9f8",
                        "surface-dim": "#dcd9d9",
                        "surface-container-high": "#eae8e7",
                        "on-background": "#1b1c1c",
                        "tertiary-fixed": "#e2e2e2",
                        "on-primary-container": "#5e002b",
                        "surface-variant": "#e4e2e1",
                        "tertiary": "#5d5f5f",
                        "surface-bright": "#fbf9f8",
                        "on-tertiary-fixed": "#1a1c1c",
                        "primary-container": "#f06292",
                        "on-tertiary-container": "#2b2d2d",
                        "on-secondary": "#ffffff",
                        "inverse-surface": "#303030",
                        "secondary-container": "#f4dce4",
                        "secondary-fixed-dim": "#d7c1c8",
                        "on-tertiary-fixed-variant": "#454747",
                        "surface-container": "#f0eded",
                        "primary-fixed": "#ffd9e1",
                        "on-primary-fixed": "#3f001b",
                        "inverse-on-surface": "#f3f0f0",
                        "on-surface": "#1b1c1c",
                        "on-primary-fixed-variant": "#8b0e45",
                        "on-primary": "#ffffff",
                        "secondary": "#6b5a60",
                        "surface-container-low": "#f6f3f2",
                        "inverse-primary": "#ffb1c5",
                        "on-secondary-fixed-variant": "#524249",
                        "primary-fixed-dim": "#ffb1c5",
                        "outline-variant": "#ddbfc5",
                        "outline": "#8a7176",
                        "tertiary-container": "#939494",
                        "error": "#ba1a1a",
                        "tertiary-fixed-dim": "#c6c6c7",
                        "on-error-container": "#93000a",
                        "on-secondary-fixed": "#25181e"
                    },
                    "borderRadius": {
                        "DEFAULT": "0.25rem",
                        "lg": "0.5rem",
                        "xl": "0.75rem",
                        "full": "9999px"
                    },
                    "spacing": {
                        "gutter": "24px",
                        "stack-sm": "12px",
                        "margin-desktop": "40px",
                        "stack-lg": "48px",
                        "base": "8px",
                        "stack-md": "24px",
                        "margin-mobile": "20px",
                        "container-max": "1280px"
                    },
                    "fontFamily": {
                        "body-md": ["Plus Jakarta Sans"],
                        "label-md": ["Plus Jakarta Sans"],
                        "display-lg-mobile": ["Playfair Display"],
                        "label-sm": ["Plus Jakarta Sans"],
                        "headline-sm": ["Playfair Display"],
                        "display-lg": ["Playfair Display"],
                        "headline-md": ["Playfair Display"],
                        "body-lg": ["Plus Jakarta Sans"]
                    },
                    "fontSize": {
                        "body-md": ["16px", { "lineHeight": "1.6", "fontWeight": "400" }],
                        "label-md": ["14px", { "lineHeight": "1.4", "letterSpacing": "0.05em", "fontWeight": "600" }],
                        "display-lg-mobile": ["32px", { "lineHeight": "1.2", "fontWeight": "700" }],
                        "label-sm": ["12px", { "lineHeight": "1.4", "fontWeight": "500" }],
                        "headline-sm": ["24px", { "lineHeight": "1.3", "fontWeight": "600" }],
                        "display-lg": ["48px", { "lineHeight": "1.2", "letterSpacing": "-0.02em", "fontWeight": "700" }],
                        "headline-md": ["32px", { "lineHeight": "1.3", "fontWeight": "600" }],
                        "body-lg": ["18px", { "lineHeight": "1.6", "fontWeight": "400" }]
                    }
                }
            }
        }
    </script>
<style>
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
    </style>
</head>
<body class="bg-background text-on-background font-body-md antialiased selection:bg-primary-container selection:text-on-primary-container">
<!-- TopNavBar -->
<header class="bg-surface dark:bg-surface-dim shadow-[0px_4px_20px_rgba(171,44,93,0.02)] sticky top-0 z-50">
<nav class="flex justify-between items-center px-margin-desktop py-4 max-w-container-max mx-auto w-full">
<div class="flex items-center gap-stack-md">
<a class="text-headline-md font-headline-md text-primary dark:text-primary-fixed-dim tracking-tight" href="#">FlowerShop</a>
<div class="hidden md:flex gap-stack-sm ml-margin-desktop">
<a class="font-label-md text-label-md text-on-surface-variant dark:text-on-surface font-medium hover:text-primary dark:hover:text-primary-fixed-dim transition-colors duration-300" href="#">Home</a>
<a class="font-label-md text-label-md text-primary dark:text-primary-fixed-dim border-b-2 border-primary dark:border-primary-fixed-dim pb-1 font-semibold opacity-80 transition-opacity" href="#">Shop</a>
<a class="font-label-md text-label-md text-on-surface-variant dark:text-on-surface font-medium hover:text-primary dark:hover:text-primary-fixed-dim transition-colors duration-300" href="#">Collections</a>
<a class="font-label-md text-label-md text-on-surface-variant dark:text-on-surface font-medium hover:text-primary dark:hover:text-primary-fixed-dim transition-colors duration-300" href="#">About</a>
<a class="font-label-md text-label-md text-on-surface-variant dark:text-on-surface font-medium hover:text-primary dark:hover:text-primary-fixed-dim transition-colors duration-300" href="#">Contact</a>
</div>
</div>
<div class="flex items-center gap-stack-sm text-primary dark:text-primary-fixed-dim">
<button aria-label="Search" class="p-2 hover:bg-surface-container rounded-full transition-colors">
<span aria-hidden="true" class="material-symbols-outlined">search</span>
</button>
<button aria-label="User Account" class="p-2 hover:bg-surface-container rounded-full transition-colors">
<span aria-hidden="true" class="material-symbols-outlined">person</span>
</button>
<button aria-label="Shopping Cart" class="p-2 hover:bg-surface-container rounded-full transition-colors relative">
<span aria-hidden="true" class="material-symbols-outlined">shopping_cart</span>
<span class="absolute top-1 right-1 w-2 h-2 bg-primary rounded-full"></span>
</button>
</div>
</nav>
</header>
<!-- Main Content -->
<main class="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-screen">
<!-- Breadcrumbs -->
<nav aria-label="Breadcrumb" class="flex text-label-sm font-label-sm text-on-surface-variant mb-stack-md">
<ol class="inline-flex items-center space-x-1 md:space-x-2">
<li class="inline-flex items-center">
<a class="hover:text-primary transition-colors" href="#">Home</a>
</li>
<li>
<div class="flex items-center">
<span class="material-symbols-outlined text-sm mx-1">chevron_right</span>
<a class="hover:text-primary transition-colors" href="#">Shop</a>
</div>
</li>
<li>
<div class="flex items-center">
<span class="material-symbols-outlined text-sm mx-1">chevron_right</span>
<a class="hover:text-primary transition-colors" href="#">Bouquets</a>
</div>
</li>
<li aria-current="page">
<div class="flex items-center">
<span class="material-symbols-outlined text-sm mx-1">chevron_right</span>
<span class="text-on-surface">Pink Rose Harmony</span>
</div>
</li>
</ol>
</nav>
<div class="grid grid-cols-1 lg:grid-cols-12 gap-margin-desktop lg:gap-gutter items-start">
<!-- Left Column: Image Gallery -->
<div class="lg:col-span-7 flex flex-col gap-stack-sm">
<div class="w-full aspect-[4/5] rounded-xl overflow-hidden shadow-[0_4px_20px_rgba(171,44,93,0.02)] bg-surface-container-low group cursor-zoom-in relative">
<img alt="Pink Rose Harmony Bouquet Main Image" class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" data-alt="A high-resolution, beautifully styled studio photograph of a large 'Pink Rose Harmony' floral bouquet. The arrangement features lush, vibrant rose pink garden roses, soft pale pink peonies, and delicate white ranunculus, loosely gathered to appear organic and romantic. The bouquet sits in a clear glass vase against a pristine white background. High-key, soft, diffused lighting highlights the velvety texture of the petals and creates a bright, modern, and elegant aesthetic typical of a luxury floral boutique. Minimal shadows are cast, emphasizing the airy feel." src="https://lh3.googleusercontent.com/aida-public/AB6AXuD52BBC0c8IZwP_2ql5qca2NRFr8KIh_ZPDklXid4Ulnsj8i__S2IwpqVDtkfQLurrX4x82NjBCdroceMv_QKXey6yd3D6eACuFWZ5X8VQ9i6fEff9zym3kX83XcLERdUjlEQYCKbKky8Vpzqd9VI5BMm8cd7KGY9E_a-ABfXfgQKyqGG3uyO6A4tWxV0E3Wdh7dwkX7XkwYeriUKUTs1IstqxdZtpNqYSX5iN29jdzxufa18BTQUKAWWb65H1cHPU5AV32NtAOR7k"/>
</div>
<div class="grid grid-cols-4 gap-stack-sm">
<div class="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-primary">
<img alt="Bouquet detail view 1" class="w-full h-full object-cover" data-alt="A close-up macro photograph focusing on the intricate details of a single vibrant rose pink garden rose within the 'Pink Rose Harmony' bouquet. Soft, natural lighting catches the dewdrops on the velvety petals. Pure white background, contemporary romance aesthetic." src="https://lh3.googleusercontent.com/aida-public/AB6AXuB9CddqN2JsuVI2rYrLjfhM9YJEFtmNp6f_UaBbD8XuKJ2qE5FLElGt1sSIizcpFIBWITclUw4cq9Zhpzs1vGtTEpSTzy6UOcn8Uf3146Ih0LPSnin2xvbSLqAc08l1_MwIKWQmPF5wXwQrMQBKupE_0bN9EZ4UW86h9zRflczjzRqvbIbsUFzIELmDwiL61nlxefYQguY7IW2PnRp72LshlZWLRnxebPiOJ0fpdgIhYGXjGLuVtDt4aPlSGy5hNcdAWNjn4O8NxTs"/>
</div>
<div class="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-transparent hover:border-outline-variant transition-colors">
<img alt="Bouquet top view" class="w-full h-full object-cover" data-alt="An overhead flat-lay photograph of the 'Pink Rose Harmony' bouquet resting on a clean, light marble surface. Soft pink ribbons are casually draped next to the stems. The lighting is bright and even, showcasing the volume and varied textures of the peonies and roses. Modern minimalist style." src="https://lh3.googleusercontent.com/aida-public/AB6AXuARzZ2qpRYpsU19whOUnfBzCxrqw3zwzDMv9zTU5J2TAmeacj0BaWjrlo-IrlQjJBljoQbGoSoIzF21u9dh3bo3b2jHjQy2W8jd31qKC3K7kd0UPuDp3iUuEIUkfpGc1e_GJKTgrk9ZcdqGWOgIDg80Ulq6XjEjJwfpZxB9zidUfXHEwrxinQBnbjR5Cly7HOst4MdMJ_fdSnZ4arKWILEdaahJabulvl9C0Ro1R3yq3W49q7veNH8L0L6P1YrTeExOXmoSnGQXuus"/>
</div>
<div class="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-transparent hover:border-outline-variant transition-colors">
<img alt="Bouquet lifestyle setting" class="w-full h-full object-cover" data-alt="A lifestyle photograph showing the 'Pink Rose Harmony' bouquet placed on a modern, light wood dining table in a sunlit, airy room with soft white curtains in the background. The scene evokes a feeling of sophisticated, everyday romance and premium home decor." src="https://lh3.googleusercontent.com/aida-public/AB6AXuA8_xORpxwk86bL3I4VJlBsIa21JwThIYnfT_IvKL09XAhTuhrJHtHVTYz3lv02Uj2dU8HpKpZNGGsh4ULoq3Sf9ZZoFSqsoCcjVd8PdjIl8hPNpARjUw9RlqbsPb4b-tUKUfv8kb8ZMBB-QxF2PzIgAvpINuwnbQKOTvnXgZioxC-lZ1b13_z8DIbBBKuD7TGNLS6RPFGP5zE5X9SyhVB7zS_3FSH5utYQwqGx-gBglQ-m9DBJBpbcNJ_mvsxXQFVdTzgZjW--ZVk"/>
</div>
<div class="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-transparent hover:border-outline-variant transition-colors bg-surface-container flex items-center justify-center">
<span class="material-symbols-outlined text-outline">play_circle</span>
</div>
</div>
<div class="mt-stack-md"><p class="font-body-md text-body-md text-on-surface-variant leading-relaxed">A symphony of soft textures and vibrant hues. The Pink Rose Harmony bouquet blends premium garden roses, lush peonies, and delicate accents to create an arrangement that speaks of contemporary romance. Perfect for anniversaries, celebrations, or simply elevating a modern space.</p></div></div>
<!-- Right Column: Product Details -->
<div class="lg:col-span-5 flex flex-col pt-2 md:pt-0 sticky top-stack-lg">
<div class="flex items-center gap-2 mb-2">
<span class="bg-secondary-container text-on-secondary-container px-3 py-1 rounded-full font-label-sm text-label-sm">Bestseller</span>
<span class="bg-surface-container text-on-surface-variant px-3 py-1 rounded-full font-label-sm text-label-sm">Spring Collection</span>
</div>
<h1 class="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-2">Pink Rose Harmony</h1>
<div class="flex items-baseline gap-4 mb-stack-md">
<p class="font-headline-md text-headline-md text-primary">$125.00</p>
<p class="font-body-md text-body-md text-on-surface-variant line-through">$145.00</p>
</div>
<!-- Configuration Options -->
<div class="space-y-stack-md mb-stack-lg">
<div>
<label class="font-label-md text-label-md text-on-surface block mb-base">Select Size</label>
<div class="grid grid-cols-3 gap-2">
<button class="border border-outline-variant rounded-lg py-3 text-center hover:border-primary hover:bg-surface-container-low transition-colors font-label-sm text-label-sm text-on-surface-variant">Classic</button>
<button class="border-2 border-primary rounded-lg py-3 text-center bg-surface-container-lowest shadow-[0_4px_20px_rgba(171,44,93,0.05)] font-label-sm text-label-sm text-primary font-semibold">Deluxe (+$30)</button>
<button class="border border-outline-variant rounded-lg py-3 text-center hover:border-primary hover:bg-surface-container-low transition-colors font-label-sm text-label-sm text-on-surface-variant">Grand (+$60)</button>
</div>
</div>
</div>
<!-- Add to Cart Area -->
<div class="flex items-center gap-stack-sm mb-stack-lg">
<div class="flex items-center border border-outline-variant rounded-lg bg-surface-container-lowest h-[52px]">
<button aria-label="Decrease quantity" class="px-4 py-2 text-on-surface-variant hover:text-primary transition-colors">
<span class="material-symbols-outlined">remove</span>
</button>
<input aria-label="Quantity" class="w-12 text-center border-none focus:ring-0 font-body-md text-body-md bg-transparent text-on-surface" min="1" type="number" value="1"/>
<button aria-label="Increase quantity" class="px-4 py-2 text-on-surface-variant hover:text-primary transition-colors">
<span class="material-symbols-outlined">add</span>
</button>
</div>
<button class="flex-1 bg-primary text-on-primary h-[52px] rounded-lg font-label-md text-label-md shadow-[0_4px_20px_rgba(171,44,93,0.2)] hover:shadow-lg hover:-translate-y-0.5 transition-all flex items-center justify-center gap-2">
<span class="material-symbols-outlined">shopping_bag</span>
                        Add to Cart
                    </button>
<button class="h-[52px] w-[52px] border border-outline-variant rounded-lg flex items-center justify-center text-on-surface-variant hover:text-primary hover:border-primary transition-colors bg-surface-container-lowest">
<span class="material-symbols-outlined">favorite_border</span>
</button>
</div>
<!-- Accordions (Details) -->
<div class="border-t border-surface-variant divide-y divide-surface-variant">
<details class="group py-4" open="">
<summary class="flex justify-between items-center font-label-md text-label-md text-on-surface cursor-pointer list-none">
                            Floral Care &amp; Handling
                            <span class="material-symbols-outlined text-outline transition-transform group-open:rotate-180">expand_more</span>
</summary>
<div class="pt-4 font-body-md text-body-md text-on-surface-variant">
<ul class="space-y-2">
<li class="flex items-start gap-2">
<span class="material-symbols-outlined text-sm text-primary mt-1">water_drop</span>
                                    Trim stems 1-2 inches at an angle upon arrival.
                                </li>
<li class="flex items-start gap-2">
<span class="material-symbols-outlined text-sm text-primary mt-1">sunny</span>
                                    Keep away from direct sunlight and drafts.
                                </li>
<li class="flex items-start gap-2">
<span class="material-symbols-outlined text-sm text-primary mt-1">local_drink</span>
                                    Change water and clean vase every 2 days.
                                </li>
</ul>
</div>
</details>
<details class="group py-4">
<summary class="flex justify-between items-center font-label-md text-label-md text-on-surface cursor-pointer list-none">
                            Delivery Information
                            <span class="material-symbols-outlined text-outline transition-transform group-open:rotate-180">expand_more</span>
</summary>
<div class="pt-4 font-body-md text-body-md text-on-surface-variant">
                            Next-day delivery available for orders placed before 2 PM local time. Bouquets are hand-delivered in our signature temperature-controlled packaging to ensure pristine condition upon arrival.
                        </div>
</details>
</div>
</div>
</div>
<!-- Related Products Section -->
<section class="mt-stack-lg pt-stack-lg border-t border-surface-variant">
<div class="flex justify-between items-end mb-stack-md">
<h2 class="font-headline-sm text-headline-sm text-on-surface">You May Also Love</h2>
<a class="font-label-sm text-label-sm text-primary hover:underline hidden sm:block" href="#">View all</a>
</div>
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-gutter">
<!-- Product Card 1 -->
<div class="group cursor-pointer">
<div class="w-full aspect-[4/5] rounded-xl overflow-hidden bg-surface-container-low mb-4 shadow-[0_4px_20px_rgba(171,44,93,0.02)] transition-shadow group-hover:shadow-[0_8px_30px_rgba(171,44,93,0.08)]">
<img alt="White Ranunculus Cloud" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" data-alt="A pristine white ranunculus bouquet in a minimalist cylindrical vase. Set against a soft pale grey background with bright, diffused lighting. The aesthetic is clean, modern, and highly sophisticated, highlighting the delicate layers of the white petals. Typical of luxury editorial floral photography." src="https://lh3.googleusercontent.com/aida-public/AB6AXuA7P_TRPbJ54vxXR3tZOHCMJdhasTDVHHFI0BYmNYjJfmF0RrY5wXQZkG1j3M2qB_CS8WmOrHt8G9LoUg-TzRYLqBswRHuHXkCFq9RzXsxnitihoj8l9pIWgx_9fzXn7SHVqR7MoalXB5-BGH12Mxn5RQmfFoZg3SUKADoszTb3r3mtru4dk3q0wTzeugv2rijxhkkqh8_CRGGgyP8Nx2gj7K7-enR_RhVpTYPUoiM_vtI2IERtC_Ci9S6ja9DS7icK60dpjNvn7Jk"/>
</div>
<div class="text-center">
<h3 class="font-label-md text-label-md text-on-surface mb-1">Pure Elegance</h3>
<p class="font-body-md text-body-md text-on-surface-variant">$95.00</p>
</div>
</div>
<!-- Product Card 2 -->
<div class="group cursor-pointer">
<div class="w-full aspect-[4/5] rounded-xl overflow-hidden bg-surface-container-low mb-4 shadow-[0_4px_20px_rgba(171,44,93,0.02)] transition-shadow group-hover:shadow-[0_8px_30px_rgba(171,44,93,0.08)]">
<img alt="Burgundy Dahlia Mix" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" data-alt="A lush bouquet of deep burgundy and soft blush dahlias mixed with dark green eucalyptus foliage. The arrangement is dramatic yet romantic, photographed in a well-lit studio setting with a clean, airy background to contrast the deep reds. Minimalist modern styling." src="https://lh3.googleusercontent.com/aida-public/AB6AXuDtfYqf9v5UvuGNk-VLBUVfGeq8FESvcpq4UePWQpPfvqO2X2pJ4S7qWkSqbq7A6rATKIsgBeypPdXCj4VBF7kaeUQTRRJQMuRmH3FhAr5U9ASBLP6nQumwKUUX3RQX2k6Ez3xFgQRTUR2JY3gf4rzpmIu7MFgagvDuiRZzC_7H_IrJLFjDQMC0LpX71y7uXYTwy1VEmrMmX2hU5zbQDFBk_DDzJjrdQOv58ykaRFyR6aGQ2RusSCvXrjJCNe5coPW5gJ1oDo_sOn4"/>
</div>
<div class="text-center">
<h3 class="font-label-md text-label-md text-on-surface mb-1">Twilight Romance</h3>
<p class="font-body-md text-body-md text-on-surface-variant">$110.00</p>
</div>
</div>
<!-- Product Card 3 -->
<div class="group cursor-pointer hidden sm:block">
<div class="w-full aspect-[4/5] rounded-xl overflow-hidden bg-surface-container-low mb-4 shadow-[0_4px_20px_rgba(171,44,93,0.02)] transition-shadow group-hover:shadow-[0_8px_30px_rgba(171,44,93,0.08)] relative">
<span class="absolute top-3 left-3 bg-surface-container-lowest text-on-surface px-2 py-1 rounded font-label-sm text-label-sm shadow-sm z-10">New</span>
<img alt="Coral Peony Arrangement" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" data-alt="A vibrant, asymmetrical floral arrangement featuring bright coral charm peonies and delicate yellow sweet peas. The style is organic and contemporary. Photographed against a bright, airy white backdrop with soft lighting to emphasize the warm, cheerful color palette. High-end modern floral design." src="https://lh3.googleusercontent.com/aida-public/AB6AXuBPXF5NWfHsofIzZCBpQ4DjXUi5kIbA1RAMffPLL_dRGZlFwMrYjcjVCY82QYAPZVm4hhz65cZjh1lQDoekdBsiSOU3TJajnw1g1PkqSIlR2gg_E95HAoDwZgKwdRPq3gVs8FARkWTebS58wM2uoesIrGnqHMdRACWGylHzHbHN8IqoiX4ImxPIVhAxwiSJLsjsH_DInanvLbtuWVyeWijHEmFHzLpK6DU7nE80E5m7YgV9B9SQMRDUK_vh2YGR_OVg5oJRcaIwc3w"/>
</div>
<div class="text-center">
<h3 class="font-label-md text-label-md text-on-surface mb-1">Sunrise Charm</h3>
<p class="font-body-md text-body-md text-on-surface-variant">$135.00</p>
</div>
</div>
<!-- Product Card 4 -->
<div class="group cursor-pointer hidden lg:block">
<div class="w-full aspect-[4/5] rounded-xl overflow-hidden bg-surface-container-low mb-4 shadow-[0_4px_20px_rgba(171,44,93,0.02)] transition-shadow group-hover:shadow-[0_8px_30px_rgba(171,44,93,0.08)]">
<img alt="Single Orchid Stem" class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105" data-alt="A minimalist presentation of a single premium stem of a Phalaenopsis orchid in a sleek glass bud vase. The setting is hyper-clean and modern, with ample negative space around the flower. The lighting is crisp and cool, accentuating the elegant structure of the bloom." src="https://lh3.googleusercontent.com/aida-public/AB6AXuD2D4I810ayAvcSa39CMCu-aTEP9ywYiMeDPVKhSPGyBrUVjiRGyK02N50NegSD_Jj5a_anFPenhwYPtr7MDTXaGjG0tinrR5CDwuCSXRoyNyv0RcnfbHZyKw8Nio808_bqjxhtJCh653h7MJ1MuYCjL9_eTPdISYJ62-KEnrq4WTeXt8AzOekndSYXweme_YUiSl_jztpSxB-HUoj47l-2fw0J8tjcduB6B7W2LGjTRLI6AbgzPRpO_OOnMxhBlJqZHvm_ElN3S4Q"/>
</div>
<div class="text-center">
<h3 class="font-label-md text-label-md text-on-surface mb-1">Minimalist Orchid</h3>
<p class="font-body-md text-body-md text-on-surface-variant">$65.00</p>
</div>
</div>
</div>
<div class="mt-6 text-center sm:hidden">
<a class="font-label-md text-label-md text-primary border border-primary rounded-lg px-6 py-2 inline-block" href="#">View all related</a>
</div>
</section>
</main>
<!-- Footer -->
<footer class="bg-surface-container-lowest dark:bg-surface-container border-t border-outline-variant dark:border-on-surface-variant">
<div class="flex flex-col md:flex-row justify-between items-center px-margin-desktop py-stack-lg max-w-container-max mx-auto w-full gap-stack-md">
<div class="text-center md:text-left">
<h2 class="font-headline-sm text-headline-sm text-primary dark:text-primary-fixed-dim mb-2">FlowerShop</h2>
<p class="font-body-md text-body-md text-on-surface-variant dark:text-on-surface">© 2024 FlowerShop. Crafted for Contemporary Romance.</p>
</div>
<nav aria-label="Footer Navigation">
<ul class="flex flex-wrap justify-center md:justify-end gap-stack-sm md:gap-stack-md">
<li><a class="font-label-sm text-label-sm text-on-surface-variant dark:text-on-surface hover:underline decoration-primary/30 transition-all focus:ring-2 focus:ring-primary/20 rounded" href="#">Privacy Policy</a></li>
<li><a class="font-label-sm text-label-sm text-on-surface-variant dark:text-on-surface hover:underline decoration-primary/30 transition-all focus:ring-2 focus:ring-primary/20 rounded" href="#">Shipping Info</a></li>
<li><a class="font-label-sm text-label-sm text-on-surface-variant dark:text-on-surface hover:underline decoration-primary/30 transition-all focus:ring-2 focus:ring-primary/20 rounded" href="#">Terms of Service</a></li>
<li><a class="font-label-sm text-label-sm text-on-surface-variant dark:text-on-surface hover:underline decoration-primary/30 transition-all focus:ring-2 focus:ring-primary/20 rounded" href="#">Floral Care Guide</a></li>
</ul>
</nav>
</div>
</footer>
</body></html>