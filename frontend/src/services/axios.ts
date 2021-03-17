import axios from 'axios';
import { token } from '@/stores/tokenStore';

const baseUrl = process.env.VUE_APP_API_BASE_URL;

if (!baseUrl) throw new Error('No VUE_APP_API_BASE_URL provided!');

const instance = axios.create({
  baseURL: baseUrl,
});

instance.interceptors.request.use((req) => {
  const tokenStr = token.value;
  if (tokenStr) {
    req.headers.Authorization = `Bearer ${tokenStr}`;
  }

  return req;
});

export default instance;
