# Stage 1: Build the Angular application
FROM node:20-alpine as build

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install all dependencies (including dev dependencies needed for build)
RUN npm ci

# Copy source code
COPY . .

# Build the application
RUN npm run build:prod

# Stage 2: Serve the application with Nginx
FROM nginx:alpine

# Copy the built application from the build stage (Angular 18+ outputs to browser subdirectory)
COPY --from=build /app/dist/wikiarea-frontend/browser /usr/share/nginx/html

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"] 