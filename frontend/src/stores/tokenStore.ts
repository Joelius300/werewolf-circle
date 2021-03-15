import { readonly, Ref, ref } from 'vue';

const tokenRef: Ref<string | null> = ref(null);
const readonlyToken = readonly(tokenRef);

function setToken(token: string | null) {
  tokenRef.value = token;

  if (token) {
    localStorage.setItem('token', token);
  } else {
    localStorage.removeItem('token');
  }
}

function initializeTokenStore() {
  const token = localStorage.getItem('token');
  if (token) {
    setToken(token);
  }
}

export { readonlyToken as token, setToken, initializeTokenStore };
