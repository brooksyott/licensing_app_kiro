<script setup lang="ts">
import AboutImage from '@/assets/About-JSI-v4.svg'
</script>

<template>
  <div class="home-view">
    <div class="content-container">
      <div class="left-section">
        <img :src="AboutImage" alt="About JSI" class="about-image" />
      </div>
      <div class="divider"></div>
      <div class="right-section">
        <div class="text-content">
          <h1 class="title">AI Licensing</h1>
          <p class="description">
            AI Licensing allows you to manage your AI features and licenses in one place.
          </p>
          <p class="description">
            Customers must be provisioned with a license to access AI features.
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-view {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
}

.content-container {
  display: flex;
  align-items: center;
  width: 100%;
  max-width: 1400px;
  gap: 3rem;
}

.left-section {
  flex: 0 0 33.333%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.about-image {
  width: 100%;
  height: auto;
  max-width: 400px;
}

.divider {
  width: 1px;
  height: 400px;
  background-color: #d3d3d3;
  flex-shrink: 0;
}

.right-section {
  flex: 1;
  display: flex;
  align-items: center;
  padding: 2rem;
}

.text-content {
  max-width: 600px;
}

.title {
  font-size: 2.5rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0 0 2rem 0;
}

.description {
  font-size: 1.25rem;
  line-height: 1.8;
  color: #555;
  margin: 0 0 1.5rem 0;
}

.description:last-child {
  margin-bottom: 0;
}

/* Responsive styles for tablet */
@media (max-width: 1024px) {
  .content-container {
    gap: 2rem;
  }

  .left-section {
    flex: 0 0 40%;
  }

  .about-image {
    max-width: 300px;
  }

  .divider {
    height: 300px;
  }

  .title {
    font-size: 2rem;
  }

  .description {
    font-size: 1.1rem;
  }
}

/* Responsive styles for mobile */
@media (max-width: 768px) {
  .home-view {
    padding: 1rem;
  }

  .content-container {
    flex-direction: column;
    gap: 2rem;
  }

  .left-section {
    flex: none;
    width: 100%;
  }

  .about-image {
    max-width: 250px;
  }

  .divider {
    width: 80%;
    height: 1px;
  }

  .right-section {
    padding: 1rem;
  }

  .text-content {
    text-align: center;
  }

  .title {
    font-size: 1.75rem;
    margin-bottom: 1.5rem;
  }

  .description {
    font-size: 1rem;
  }
}
</style>
