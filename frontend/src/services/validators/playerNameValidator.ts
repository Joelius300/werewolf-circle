export default function isValidPlayerName(username: string | null | undefined) {
  return username != null && username.match('^[\\w\\-]{2,25}$') != null;
}
