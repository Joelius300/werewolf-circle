FROM node:latest as build-stage
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY ./ .
RUN npm run build

FROM alpine:latest as production-stage
COPY --from=build-stage /app/dist /app
VOLUME [ "/app" ]
