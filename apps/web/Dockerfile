FROM node:22-alpine AS build

WORKDIR /app

RUN npm install -g pnpm@10.28.2

COPY package.json pnpm-lock.yaml ./

RUN pnpm install --frozen-lockfile

COPY . .

RUN sed -i '/"@nx\/dotnet"/d' nx.json

RUN npx nx build web --configuration=production

FROM nginx:alpine

COPY --from=build /app/dist/apps/web/browser /usr/share/nginx/html

COPY apps/web/nginx.conf /etc/nginx/conf.d/default.conf

RUN chown -R nginx:nginx /var/cache/nginx /var/log/nginx /etc/nginx/conf.d && \
  chmod -R 755 /usr/share/nginx/html && \
  touch /var/run/nginx.pid && \
  chown nginx:nginx /var/run/nginx.pid

USER nginx

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD test -f /var/run/nginx.pid || exit 1

CMD ["nginx", "-g", "daemon off;"]
