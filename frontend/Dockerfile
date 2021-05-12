FROM node:14 as build-stage
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY ./ .

ARG VUE_APP_API_BASE_URL
ARG VUE_APP_GAME_HUB_URL

RUN npm run build

FROM alpine:latest as production-stage
COPY --from=build-stage /app/dist /app
VOLUME [ "/app" ]
