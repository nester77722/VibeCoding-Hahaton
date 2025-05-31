<template>
  <div class="not-found">
    <h1>404</h1>
    <h2>Page Not Found</h2>
    <p>The page you're looking for doesn't exist or has been moved.</p>
    <router-link to="/" class="home-link">Go to Home</router-link>
  </div>
</template>

<style scoped>
.not-found {
  max-width: 600px;
  margin: 4rem auto;
  text-align: center;
  padding: 2rem;
}

h1 {
  font-size: 6rem;
  color: #2ed573;
  margin: 0;
  line-height: 1;
}

h2 {
  font-size: 2rem;
  color: #2c3e50;
  margin: 1rem 0;
}

p {
  color: #57606f;
  margin-bottom: 2rem;
}

.home-link {
  display: inline-block;
  background: #2ed573;
  color: white;
  text-decoration: none;
  padding: 1rem 3rem;
  border-radius: 6px;
  font-size: 1.1rem;
  transition: background 0.3s ease;
}

.home-link:hover {
  background: #7bed9f;
}
</style> 