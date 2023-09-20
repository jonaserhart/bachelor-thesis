FROM node:19-alpine AS build
WORKDIR /build

COPY package.json ./
COPY yarn.lock ./
RUN yarn install --frozen-lockfile

COPY . ./
RUN yarn build

FROM nginx:alpine AS runtime

COPY .env /usr/share/nginx/html/.env

RUN apk add --update nodejs
RUN apk add --update npm
RUN npm install -g runtime-env-cra@0.2.2

COPY --from=build /build/build/ /usr/share/nginx/html

WORKDIR /usr/share/nginx/html

COPY ./nginx.conf /etc/nginx/conf.d/default.conf

CMD ["/bin/sh", "-c", "runtime-env-cra && nginx -g \"daemon off;\""]