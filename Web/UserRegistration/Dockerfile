# # Stage 1: Build the Angular application
# FROM node:latest AS build
# WORKDIR /app
# COPY package*.json ./
# RUN npm install
# COPY . .
# RUN npm run build --prod

# # Stage 2: Serve the Angular application with Nginx
# FROM nginx:alpine
# COPY --from=build /app/dist/user-registration /usr/share/nginx/html
# EXPOSE 8080
# # CMD ["nginx", "-g", "daemon off;"]

# Stage 1: Compile and Build angular codebase


### STAGE 1: Build ###
FROM node:lts-alpine AS build

#### make the 'app' folder the current working directory
WORKDIR /usr/src/app

#### copy both 'package.json' and 'package-lock.json' (if available)
COPY package*.json ./

#### install angular cli
RUN npm install -g @angular/cli

#### install project dependencies
RUN npm install

#### copy things
COPY . .

#### generate build --prod
RUN npm run build --prod

### STAGE 2: Run ###
FROM nginxinc/nginx-unprivileged

#### copy nginx conf
COPY ./nginx.conf /etc/nginx/conf.d/default.conf

#### copy artifact build from the 'build environment'
COPY --from=build /usr/src/app/dist/user-registration /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]