FROM node:current-alpine3.21

WORKDIR /app

COPY package*.json ./

RUN npm install


COPY . .

EXPOSE 4200

CMD ["npm","run", "start","--","--host", "0.0.0.0"]