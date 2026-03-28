# Web Frontend

Vue 3 + TypeScript + Vite + Tailwind CSS + PrimeVue frontend application for Perpendicularity.

## Tech Stack

- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vite** - Next-generation frontend tooling
- **Tailwind CSS** - Utility-first CSS framework
- **PrimeVue** - Rich UI component library
- **PrimeIcons** - Icon library

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

```bash
# Install dependencies
npm install
```

### Development

```bash
# Start development server
npm run dev

# The app will be available at http://localhost:62003/
```

### Build for Production

```bash
# Build the application
npm run build

# Preview the production build
npm run preview
```

## Project Structure

```
Web/
├── src/
│   ├── App.vue          # Main application component
│   ├── main.ts          # Application entry point
│   ├── style.css        # Global styles with Tailwind directives
│   └── env.d.ts         # TypeScript declarations
├── public/              # Static assets
├── index.html           # HTML entry point
├── vite.config.ts       # Vite configuration
├── tailwind.config.js   # Tailwind CSS configuration
├── tsconfig.json        # TypeScript configuration
└── package.json         # Project dependencies
```

## API Integration

The Vite development server is configured with a proxy to forward API requests to the .NET backend:

- Frontend: `http://localhost:62003/`
- API Proxy: Requests to `/api/*` are forwarded to `http://localhost:5000/api/*`

## Features

- ✅ Vue 3 Composition API with `<script setup>`
- ✅ TypeScript support
- ✅ Tailwind CSS for styling
- ✅ PrimeVue components (Button, Card, etc.)
- ✅ Dark mode support
- ✅ Hot Module Replacement (HMR)
- ✅ API proxy configuration for backend integration

## Customization

### Tailwind CSS

Customize the Tailwind configuration in `tailwind.config.js` to add custom colors, fonts, and other design tokens.

### PrimeVue Theme

The app uses PrimeVue's Lara Light Blue theme. You can change this by importing a different theme in `src/main.ts`:

```typescript
import 'primevue/resources/themes/lara-dark-blue/theme.css' // or any other theme
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build locally
