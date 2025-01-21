# FROM node:alpine

# COPY package*.json ./
# WORKDIR /app
# # COPY . /app

# RUN npm install -g @angular/cli

# RUN npm install
# EXPOSE 4200
# CMD ["ng", "serve", "--host", "0.0.0.0"]

# Use the Node.js base image
FROM node:22

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./
COPY decorate-angular-cli.js ./

# Install dependencies
RUN npm install

# Copy the entire Angular app source code to the container
COPY . .

# Expose the default Angular development server port
EXPOSE 4200

# Start the Angular app with ng serve
CMD ["npm", "run", "start"]