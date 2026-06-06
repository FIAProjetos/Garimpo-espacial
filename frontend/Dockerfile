FROM node:20-alpine

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

EXPOSE 8081 19000 19001 19002

ENTRYPOINT ["sh", "docker-entrypoint.sh"]
CMD ["npx", "expo", "start", "--web", "--host", "lan"]
