export default function isValidPlayerName(username: string) {
  return username != null && username.match('^[\\w\\-]{2,25}$') != null;
}
