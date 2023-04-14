FROM node:18-alpine AS build
WORKDIR /build

COPY package.json ./
COPY yarn.lock ./
RUN yarn

COPY . ./
RUN yarn build

FROM nginx:alpine
COPY --from=build /build/build/ /usr/share/nginx/html